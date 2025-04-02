# Event Manager Web API

The Event Manager Web API is a C# .NET application designed to manage event-related functionalities such as creating, updating, and tracking events. Built on .NET 8.0, this application leverages Entity Framework Core for SQLite database interaction, ensuring efficient data management.

Key features include:
- **ASP.NET Core** for building the web API and handling HTTP requests.
- **Entity Framework Core** with **SQLite** for seamless data management, including migration handling.
- **JWT Bearer Authentication** for secure, token-based API access.
- **Scalable architecture** built on modern web standards with ASP.NET Core.
- **Swagger UI** integration for easy API documentation and testing.

This application offers a robust and secure backend solution for event-driven applications, utilizing the latest .NET technologies for optimal performance and ease of use.

# How to start

Firstly, create a database by running the command: `dotnet ef database update`.
If successful, a database will be created in Database folder with initial events and **admin user**.

Admin user login data:
- username: `admin`
- password: `admin`

With admin privileges, you can change user role (`Admin`, `Organizer` and `User`) and user status (`Approved` and `Pending`) to give them to give them accordingly privileges.

## Roles

### Admin (top level privileges user)
- can update users
- can get events
- can post events
- can update his events
- can delete events
- can participate in event

### Organizer
- can get events
- can post events
- can update his events
- can delete his events
- can participate in event

### User
- can get events
- can participate in event

## Statuses

### Approved
- can get events
- can post events
- can update his events
- can delete his events
- can participate in event

### Pending
- can get events
- can participate in event


# Event Manager API Documentation

This API allows users to manage events, including creating, updating, deleting, and participating in events. The API also supports role-based authorization to ensure only authorized users can perform certain actions.

## Endpoints

### Authentication Controller (`AuthController`)

### 1. `POST: /api/auth/register`

**Description:**
Registers a new user.

**Body Parameters:**
- `Username` (required): The username of the new user.
- `Password` (required): The password of the new user.
- `Email` (required): The email address of the new user.

**Authorization:**
- **[AllowAnonymous]**: This endpoint is available to all users without requiring authentication.

**Response:**
- **200 OK**: The user is successfully registered and returned.
- **400 Bad Request**: If the username already exists in the database.

### 2. `POST: /api/auth/login`

**Description:**
Logs in a user and returns an authentication token.

**Body Parameters:**
- `Username` (required): The username of the user.
- `Password` (required): The password of the user.

**Authorization:**
- **[AllowAnonymous]**: This endpoint is available to all users without requiring authentication.

**Response:**
- **200 OK**: Returns an authentication token (`accessToken` and `refreshToken`).
- **400 Bad Request**: If the username or password is invalid.

### 3. `POST: /api/auth/refresh-token`

**Description:**
Refreshes an expired access token using a valid refresh token.

**Body Parameters:**
- `refreshToken` (required): The refresh token to generate a new access token.

**Authorization:**
- **[AllowAnonymous]**: This endpoint is available to all users without requiring authentication.

**Response:**
- **200 OK**: Returns the newly generated `accessToken` and `refreshToken`.
- **401 Unauthorized**: If the provided refresh token is invalid.

### 4. `POST: /api/auth/update-user`

**Description:**
Updates the role and status of an existing user.

**Body Parameters:**
- `Username` (required): The username of the user to update.
- `Role` (required): The new role for the user. (`Admin`, `Organizer`, or `User`).
- `Status` (required): The new status for the user. (`Approved` or `Pending`).

**Authorization:**
- **[AuthorizeRoles(UserRole.Admin)]**: This endpoint requires the user to be an **Admin**.

**Response:**
- **200 OK**: The user was successfully updated.
- **400 Bad Request**: If the user does not exist or if the role/status is invalid.
- **403 Forbidden**: If the user does not have the necessary permissions (only Admins can update users).

### Authorization and Roles

- **[AllowAnonymous]**: This attribute is used to indicate that the endpoint can be accessed without authentication, for example, during registration, login, or token refresh.
- **[Authorize]**: Used to indicate that the endpoint is protected and only accessible by authenticated users with a valid JWT token.
- **[AuthorizeRoles(UserRole)]**: A custom role-based authorization attribute that restricts the access of specific actions to users with the `Admin`/`Organizer`/`User` role.

### Events Controller (`EventsController`)

### 1. `GET: /api/events`

**Description:**
Retrieves a list of all events.

**Query Parameters:**
- `order` (optional): Determines the sort order of the events. Can be either `asc` or `desc`.
- `searchTerm` (optional): A search term to filter events by title. This is a case-insensitive search.
- `pageSize` (optional): Specifies the number of events to return per page. Defaults to 10 if not specified.
- `page` (optional): The page number to retrieve. Used in combination with `pageSize` to paginate results.

**Authorization:**
- **[Authorize]**: This endpoint is authorized for all authenticated users.

**Response:**
- **200 OK**: A list of events based on the specified filters, including pagination and sorting.
- **500 Internal Server Error**: If an error occurs during the database query.

---

### 2. `GET: /api/events/{id}`

**Description:**
Retrieves the details of a specific event by its ID.

**Parameters:**
- `id` (required): The unique identifier of the event to retrieve.

**Authorization:**
- **[Authorize]**: This endpoint is authorized for all authenticated users.

**Response:**
- **200 OK**: The event data is returned in the response body.
- **404 Not Found**: If no event with the specified ID exists.

---

### 3. `POST: /api/events`

**Description:**
Allows authorized users to create a new event.

**Body Parameters:**
- `Title` (required): The title of the event.
- `Description` (required): A description of the event.
- `Date` (required): The date and time when the event is scheduled.
- `Location` (required): The location of the event.

**Authorization:**
- **[AuthorizeRoles(UserRole.Admin, UserRole.Organizer)]**: This endpoint is authorized for users with the roles `Admin` or `Organizer` only.

**Response:**
- **201 Created**: The event is created successfully. Returns the created event data.
- **403 Forbidden**: If the user is not approved to create events (i.e., the user�s status is not approved).
- **400 Bad Request**: If the request body is invalid or required parameters are missing.

---

### 4. `PUT: /api/events/{id}`

**Description:**
Updates the details of an existing event.

**Parameters:**
- `id` (required): The unique identifier of the event to update.

**Body Parameters:**
- `Id` (required): The unique identifier of the event (must match the `id` in the URL).
- `Title` (required): The updated title of the event.
- `Description` (required): The updated description of the event.
- `Date` (required): The updated date and time of the event.
- `Location` (required): The updated location of the event.

**Authorization:**
- **[Authorize]**: This endpoint is authorized for all authenticated users.
- **Method Restriction**: Only the event creator (the user who created the event) is allowed to update the event. If the current user is not the event creator, the request will be denied with a **405 Method Not Allowed** status.

**Response:**
- **204 No Content**: The event was successfully updated.
- **404 Not Found**: If no event with the specified ID exists.
- **405 Method Not Allowed**: If the event was not created by the current user (i.e., only the event creator can update the event).

---

### 5. `DELETE: /api/events/{id}`

**Description:**
Allows users to delete an event by its ID.

**Parameters:**
- `id` (required): The unique identifier of the event to delete.

**Authorization:**
- **[Authorize]**: This endpoint is authorized for all authenticated users.
- **Method Restriction**: Only the event creator or an `Admin` role user can delete the event. If the current user is not the event creator and does not have the `Admin` role, the request will be denied with a **405 Method Not Allowed** status.

**Response:**
- **204 No Content**: The event was successfully deleted.
- **404 Not Found**: If no event with the specified ID exists.
- **405 Method Not Allowed**: If the event was not created by the current user and the user is not an admin.

---

### 6. `POST: /api/events/{id}/participate`

**Description:**
Allows a user to participate in an event by adding them to the event's participant list or if he already is on list, removes him.

**URL:** `/api/events/{id}/participate`

**Method:** `POST`

**Parameters:**
- `id` (required): The unique identifier of the event the user wants to participate in.

**Authorization:**
- **[Authorize]**: This endpoint is authorized for all authenticated users.

**Response:**
- **200 OK**: The user is successfully added as a participant in the event.
- **200 OK**: The user is successfully removed as a participant in the event.
- **400 Bad Request**: If the user is already a participant in the event.
- **404 Not Found**: If the event with the specified `id` does not exist.
- **403 Forbidden**: If the user is not allowed to participate (e.g., the event might be full, or other conditions that prevent participation).
- **500 Internal Server Error**: If an error occurs during the database operation.
