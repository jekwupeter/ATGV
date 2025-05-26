# ATGV
Access Token Generator &amp; Verifier

## 1. About The Project
This project provides a robust, secure backend service for managing user authentication and generating time-bound access tokens. It's designed to handle user registration, login, JWT-based session management, and the creation/validation of custom 6-digit alphanumeric access tokens for specific functionalities.

## 2. Features
### User Registration (Sign Up):
Users can sign up using a unique username (must be a valid email address) and a password.
A confirmation email is sent to the registered email address to verify the user's identity and activate the account.
### User Authentication (Login):
Users can sign in using their registered email and password.
Upon successful login, a JSON Web Token (JWT) is returned for subsequent authenticated API calls.
### Access Token Generation:
Authenticated users can generate a unique 6-digit alphanumeric access token.
The token's lifespan is dynamically determined at generation time, but is capped at a maximum of 3 days from the current date. The expiry date cannot be in the past.
Tokens are securely associated with the generating user.
### Access Token Verification:
Provides an endpoint to validate generated 6-digit access tokens by the currently logged-in user.

## 3. Technologies Used
- C# (.NET Core 9)
- JWT (JSON Web Tokens)
- Entity Framework Core
- Swagger

## 4 TODO
- add opentelemetry
- add Dockerfile 
