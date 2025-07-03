
# **Wrok: The Freelance Marketplace Backend**

This project is a **freelance marketplace platform**, built using **.NET Core**, **.NET Aspire**, **Entity Framework**, **PostgreSQL**, and **Redis**. It follows **Domain-Driven Design (DDD)** principles to ensure maintainable, scalable, and testable architecture. The system is also designed to handle complex features such as **multitenancy**, **messaging**, **caching**, and **role-based authentication**.

---

## **Key Features**

* **Multitenancy**: Tenants (companies) can create and manage their own projects, bids, freelancers, and reviews.
* **Role-based Authentication**: Users can have different roles within a tenant (Admin, Project Manager, Freelancer) with fine-grained access control.
* **Project Management**: Tenants can create projects, assign them to freelancers, and review the work done.
* **Bidding System**: Freelancers can place bids on open projects. Tenants can accept or reject bids.
* **Freelancer Reviews**: After project completion, tenants can rate and review freelancers based on their performance.
* **Skill Management**: Tenants define their own skill catalog to match project requirements with freelancer expertise.
* **Messaging & Notifications**: Freelancers and tenants are notified of bid acceptances, status changes, and reviews.
* **Caching**: Redis is used for caching frequently accessed data for better performance.

---

## **Tech Stack**

* **.NET Core 6+**: Backend framework for building the API.
* **Entity Framework Core**: ORM for database interactions with PostgreSQL.
* **PostgreSQL**: Relational database for data persistence.
* **Redis**: Caching layer for frequently used data.
* **YARP**: Reverse proxy to separate the authentication API from the normal API for better scalability and security.
* **Swagger**: API documentation.
* **JWT Authentication**: Secure, role-based authentication using JWT tokens.

---

## **Architecture Overview**

The application follows **Domain-Driven Design (DDD)** principles to structure the project in a way that the domain model is at the center of business logic. Key elements of the architecture include:

* **Orchestration**: The application is managed with .NET Aspire.
* **Core Domain**: Contains the aggregates (Project, Bid, Assignment, etc.) and business rules. This layer is independent of any external systems.
* **Application Layer**: Coordinates domain operations like creating a project, bidding on a project, etc., ensuring that business rules are respected.
* **Infrastructure Layer**: Manages persistence (using Entity Framework and PostgreSQL), caching (via Redis), and external integrations (e.g., messaging or auth services).
* **Presentation Layer**: Exposes an API that interacts with clients (e.g., web or mobile apps), using controllers and models.
* **Event-Driven Architecture**: Domain events are used to communicate changes in state across different components.

---

## **Business Rules**

* **Projects**:

  * Projects are tenant-specific and must be created by users with appropriate permissions (Admin or Project Manager).
  * Projects must have a title, description, at least one required skill, and a positive budget.
  * The status of projects can transition between "Draft", "Open", "InProgress", "Completed", and "Closed".

* **Bidding**:

  * Freelancers can place a bid on a project that is "Open".
  * Only one bid per freelancer per project is allowed.
  * A bid can be accepted by a tenant, leading to the assignment of a freelancer to the project.

* **Reviews**:

  * After a project is completed, tenants can leave a review for the freelancer. Reviews are immutable and are linked to the project assignment.

* **Skills**:

  * Tenants can define and manage a custom catalog of skills.
  * Projects require a set of skills to be defined before they can be opened for bidding.

* **Roles & Permissions**:

  * Users can have different roles within a tenant (e.g., Admin, Project Manager, Freelancer) with different permissions on what they can and cannot do.

---

## **Project Structure**

The project is structured into several layers to maintain separation of concerns:

* **Domain Layer**: Contains the core business logic (aggregates, entities, value objects, domain services).
* **Application Layer**: Coordinates the use cases (services, handlers).
* **Infrastructure Layer**: Provides database access (repositories), caching, external service integrations.
* **Web API Layer**: Provides API endpoints for interacting with the platform.

---

## **Getting Started**

To run the project locally, follow these steps:

### Prerequisites:

* .NET 9 or higher
* Docker Desktop (for Redis, DB, etc.)

### Setup Instructions:

1. Clone the repository:

   ```bash
   https://github.com/CodeINN95612/Wrok
   ```

2. Build and run the application

3. The Aspire dashboard will be launched

---

## 🛠️ **Future Improvements**

* **Messaging/Notifications**: Integrating a messaging system (e.g., RabbitMQ) for real-time updates between tenants and freelancers.
* **Payment Integration**: Adding a payment gateway for project billing, freelancer compensation, etc.
* **Analytics**: Creating a reporting system for tenants to view project statistics, freelancer performance, etc.
* **Microservices**: Breaking out components (e.g., authentication, messaging) into microservices.

---

## 👨‍💻 **Contributing**

We welcome contributions! To get started:

1. Fork the repository.
2. Create a new branch for your feature.
3. Commit your changes.
4. Open a pull request to the `main` branch.

---

## 📄 **License**

This project is licensed under the MIT License.
