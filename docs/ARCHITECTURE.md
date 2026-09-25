# Architecture

MiniChat is a WhatsApp-style chat app: a .NET MAUI client talking to an ASP.NET Core Web API over
REST (for CRUD) and SignalR (for realtime message delivery). Both projects live in one solution,
[`ChatApp.slnx`](../ChatApp.slnx), so the domain models and DTO contracts are easy to keep in sync
by hand while iterating.

```
ChatApp/
├── ChatApp.slnx
├── src/
│   ├── ChatApp.Api/         REST + SignalR backend (ASP.NET Core 10)
│   └── ChatApp.Client/      MAUI 10 client (Android, iOS, MacCatalyst, Windows)
└── docs/
```

## Backend — `ChatApp.Api`

### Layering

Requests flow top to bottom; each layer only knows about the one below it.

```
Controllers / ChatHub   (presentation — HTTP + realtime)
        |
     Services            (application logic, authorization checks, mapping)
        |
  Repositories / UnitOfWork   (data access abstraction)
        |
   AppDbContext (EF Core)     (infrastructure)
        |
       Models                 (domain entities)
```

- **Controllers** (`Controllers/AuthController.cs`, `UsersController.cs`, `ConversationsController.cs`)
  are thin: they resolve the current user from the JWT claims, call a service, and translate the
  result into an HTTP response. No EF Core or business logic lives here.
- **`ChatHub`** (`Hubs/ChatHub.cs`) is the realtime counterpart — it calls the same
  `IConversationService`/`IMessageService` methods the REST controllers use, so a message sent
  over SignalR and a message sent over REST go through identical validation and persistence code.
- **Services** (`Services/*.cs`) hold the actual application logic: registering/authenticating a
  user, finding-or-creating a 1:1 conversation, sending a message, marking messages read. They
  depend on `IUnitOfWork`, never on `AppDbContext` directly.
- **Repositories + Unit of Work** (`Repositories/*.cs`) wrap `AppDbContext` behind
  `IRepository<T>`/`IUnitOfWork` so services talk to a small, testable data-access surface instead
  of a full `DbContext`.
- **Models** (`Models/*.cs`) are plain EF Core entities — `User`, `Conversation`,
  `ConversationUser`, `Message` — configured via `IEntityTypeConfiguration<T>` classes in
  `Data/Configurations/` rather than data annotations, keeping persistence concerns out of the
  domain classes themselves.

### Cross-cutting patterns

| Pattern | Where | Why |
|---|---|---|
| DTO | `DTOs/**/*.cs` | Domain entities (with `PasswordHash`, EF navigation properties) are never serialized to clients. Every controller/hub method returns or accepts a DTO. |
| Repository + Unit of Work | `Repositories/*.cs` | Isolates services from EF Core specifics and gives one `SaveChangesAsync()` per logical operation. |
| Observer | `Hubs/ChatHub.cs` | SignalR groups (`conversation-{id}`) are the "subject"; connected clients are "observers" notified via `Clients.Group(...).SendAsync(...)`. |
| JWT Bearer Authentication | `Program.cs`, `Services/TokenService.cs` | Stateless auth; the same token authenticates REST calls (via the `Authorization` header) and the SignalR hub (via an `access_token` query string, since browser/native WebSocket clients can't set custom handshake headers). |
| RESTful Controllers | `Controllers/*.cs` | Conventional resource-oriented routes: `/api/auth`, `/api/users`, `/api/conversations`. |

### Data flow example: sending a message

1. Client calls `SendMessageAsync(conversationId, text)` on its `SignalRService`, which invokes
   the hub method `ChatHub.SendMessage`.
2. `ChatHub` checks the caller is a participant (`IConversationService.IsParticipantAsync`), then
   calls `IMessageService.SendAsync`, which creates a `Message` via `IUnitOfWork.Messages` and
   commits with `SaveChangesAsync()`.
3. `ChatHub` broadcasts the resulting `MessageDto` to every connection in the
   `conversation-{id}` SignalR group via `Clients.Group(...).SendAsync("ReceiveMessage", message)`.
4. Every client's `SignalRService.MessageReceived` event fires; the open `ChatViewModel` for that
   conversation appends the message to its `ObservableCollection<MessageModel>`, and the
   `ChatsViewModel` (if the chats list is visible) updates that conversation's preview.

## Frontend — `ChatApp.Client`

### MVVM

- **Views** (`Views/*.xaml`) are passive — they bind to a ViewModel and never contain business
  logic. `x:DataType` is set on every page and `DataTemplate` for compiled bindings.
- **ViewModels** (`ViewModels/*.cs`) extend `BaseViewModel : ObservableObject` and use
  CommunityToolkit.Mvvm source generators — `[ObservableProperty]` for bindable state,
  `[RelayCommand]` for commands. `ChatViewModel` additionally implements `IQueryAttributable` to
  receive its `conversationId` route parameter as a typed value rather than a query string.
- **Models** (`Models/*.cs`) are plain client-side DTOs mirroring the API's DTOs, plus a couple of
  client-only fields (`MessageModel.IsMine`, `ConversationModel.OtherParticipantsDisplay`) that
  only make sense once you know who's logged in — set by the ViewModel, never serialized.

### Validation

`Login`/`RegisterViewModel` expose form fields as `ValidatableObject<string>`
(`Validation/ValidatableObject.cs`) instead of plain strings — each one wraps a `Value` plus a list
of `IValidationRule<T>` (`Validation/IValidationRule.cs`; `IsNotNullOrEmptyRule<T>` and
`MinLengthRule` are the two concrete rules in use). `Validate()` runs every attached rule and
populates `IsValid`/`Error`.

Validation is triggered two ways:
- **Manually** — `LoginAsync`/`RegisterAsync` call a private `ValidateForm()` that runs
  `Validate()` on every field before attempting the API call, aborting if any field fails.
- **On property change** — once a field has failed validation once, `ValidatableObject<T>`
  re-validates on every subsequent edit (via the CommunityToolkit.Mvvm-generated
  `OnValueChanged` hook), so its error clears the moment the user fixes it rather than only on
  the next submit attempt.

Views bind directly to the nested properties — `Text="{Binding Username.Value}"`, an error `Label`
bound to `Username.Error`/`Username.IsValid`, and a `DataTrigger` that turns the `Entry`'s
background pink while `IsValid` is `False` — so highlighting an invalid control and displaying its
message both fall out of the same two bindings, no extra wiring per field.

### Services

| Service | Responsibility |
|---|---|
| `ApiService` (`Services/ApiService.cs`) | Typed `HttpClient` wrapper over the REST endpoints; holds the bearer token and attaches it to every request. |
| `SignalRService` (`Services/SignalRService.cs`) | Wraps a `HubConnection`; exposes `MessageReceived`/`MessagesRead` events instead of leaking SignalR types into ViewModels. |
| `AuthService` (`Services/AuthService.cs`) | **Facade** over `ApiService` + `SignalRService` + `SecureStorage`: login/register persist the token and current user to secure storage and open the SignalR connection; `TryRestoreSessionAsync` rehydrates a session on app start. |
| `MauiNavigationService` (`Services/Navigation/MauiNavigationService.cs`) | Implements `INavigationService`, the sole thing ViewModels depend on to navigate — see *Navigation* below. |

### Dependency injection

Registered in `MauiProgram.cs`:
- **Singleton**: `IApiService`, `ISignalRService`, `IAuthService`, `INavigationService`, `AppShell`
  — one instance for the life of the app, since they hold connection/session state (or, for
  `MauiNavigationService`, are stateless enough not to need a new instance per page).
- **Transient**: every ViewModel and Page — a fresh instance per navigation, which is what lets
  `IQueryAttributable` on `ChatViewModel` bind cleanly to whichever conversation was opened.

### Navigation

`AppShell.xaml` hosts a `TabBar` with three tabs — Chats, Contacts, Settings. `LoginPage` is a
top-level `ShellContent` sibling of that `TabBar` (not inside it), so absolute (`//`) navigation
can target it — Shell doesn't allow absolute navigation onto a route that isn't part of the visual
tree. `RegisterPage` and `ChatPage` stay outside the visual tree entirely, registered as detached
routes in `AppShell.xaml.cs` via `Routing.RegisterRoute`, since they're only ever pushed relatively
on top of an existing page and never targeted by `//`.

ViewModels never call `Shell.Current.GoToAsync` directly — they depend on
`INavigationService` (`Services/Navigation/INavigationService.cs`), implemented by
`MauiNavigationService`, which wraps Shell behind three methods:

- `GoToAsync(route)` — e.g. `"//ChatsPage"` after login, `"//LoginPage"` after logout.
- `GoToAsync(route, IDictionary<string, object> parameters)` — passes typed values (not stringified
  query params) to the target page's `IQueryAttributable.ApplyQueryAttributes`. `ContactsViewModel`
  uses this after an async REST call resolves the conversation id it needs to navigate to.
- `GoBackAsync()`.

For navigation that needs **no async work first** — e.g. tapping an existing conversation in
`ChatsPage` — a `NavigateBehavior` (`Behaviors/NavigateBehavior.cs`) triggers it declaratively from
XAML instead of a command, resolving `INavigationService` from `MauiContext.Services` since
behaviors are constructed by the XAML parser rather than the DI container.

Any page's ViewModel can implement `IConfirmNavigation` to be asked before navigation away from it
is allowed to proceed — `ChatViewModel` uses this to prompt when leaving with an unsent draft.
`AppShell` hooks `Shell.Navigating` (not just its own `INavigationService`) so this catches *every*
way navigation can be triggered — hardware back, the toolbar back arrow, or an explicit
`GoToAsync` — using `ShellNavigatingEventArgs.GetDeferral()` to run the async confirmation before
Shell decides whether to actually navigate.

See [`docs/PATTERN_GUIDE.md`](PATTERN_GUIDE.md) for a file/line index of every pattern mentioned
above.
