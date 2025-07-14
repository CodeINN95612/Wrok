# Version Log

## 0.1

### 0.1.0
- Login functionality
- Refresh token functionality
- DDD
- CQRS
- Entities
- Register setup
- Users endpoints setup
- JWT Generation

### 0.1.1

- Add version log and assembly versioning (for some reason)
- Register functionality

## 0.2

### 0.2.0

- Users Get All
- Users Get by Id

## 0.3

### 0.3.0

- Jwt Validation, Auth, Authz

## 0.4

### 0.4.0

- Identity Provider
- Tenant Joining with role (generate invitation)

### 0.4.1

- Add invited user to tenant
- Fixed other stuff in invites
- Add tenant validation to users features

### 0.4.2

- Add Custom Error Codes

## 0.5

### 0.5.0

- Identity Domain Tests
- Some fixes

### 0.5.1

- Application tests

### Doing

- Unit Tests

## TODO

- Ids to record structs if possible lets see.
- Email Confirmation
- Email invitation
- DateTime provider for testing
- Group Endpoints
- Apply Row Level Security with EF interceptors
- Extension for FluentValidation Error to ErrorOr
- Add Scalar for OpenApi doc?
- Pagination?
- Deployment
    - Postgresql flexible azure deployment
    - MediatR license
- SHould JWT Generation be on infrastructure layer?