# SurveyBasket

SurveyBasket is a **modern, production-grade survey management system** built on **ASP.NET Core (.NET 9)** using **Clean Architecture**, **CQRS**, and **Domain-Driven Design** principles.  
It provides a robust, extensible platform for creating, distributing, and analyzing surveys — featuring **JWT authentication**, **background job scheduling**, **hybrid caching**, **Mapster-based object mapping**, and **containerized deployment**.

---

## 🧭 Table of Contents
- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Background Jobs](#background-jobs)
- [Authentication](#authentication)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)
- [Author](#author)

---

## 📖 Overview

**SurveyBasket** is a full-featured web backend for survey creation, management, and analytics.  
Built using **Clean Architecture** and **DDD**, it enforces clear separation of concerns and high maintainability.  

It integrates **advanced enterprise patterns** like:
- Repository & Result patterns for consistency and testability  
- Hybrid caching with both in-memory and Redis
  Professional Login with Cerilog and Seq
- Centralized exception handling with Problem Details  
- FluentValidation for robust model validation  
- Secure token-based authentication and refresh flows  

---

## ⚙️ Key Features

- **Authentication & Security**
  - JWT + Refresh Tokens + OAuth2 integration
  - Role-based and policy-based authorization
  - Integration with Azure Key Vault for secret management  

- **Survey Management**
  - Create, update, delete, publish, restore surveys
  - Rich question types and conditional logic
  - Role-restricted administration endpoints  

- **Analytics**
  - Statistical and raw data endpoints
  - Submission aggregation and visualization hooks  

- **Background Jobs**
  - Powered by **Hangfire**
  - Handles emails, cleanup, and scheduled survey processing  

- **Caching**
  - **Hybrid Caching**: combines in-memory and Redis for optimal performance  
  - Distributed cache invalidation  

- **Validation**
  - **FluentValidation** for request and model validation  
  - Strongly typed configuration and environment-based settings  

- **Performance & Observability**
  - Rate limiting and response compression  
  - **Serilog**, **Seq**, **Prometheus**, and **Grafana** integration  

- **DevOps & Infrastructure**
  - Dockerized microservices
  - GitHub Actions CI/CD pipelines  
  - Environment-based configuration for staging and production  

- **Error Handling**
  - Centralized middleware implementing RFC-7807 Problem Details  

- **API Management**
  - OpenAPI (Swagger)
  - API versioning and health checks  

---

##  Tech Stack

### 🖥 Languages & Frameworks
- **C#**, **ASP.NET Core (.NET 9)**  
- **SQL Server**, **PostgreSQL**, and **T-SQL**

### Architecture & Patterns
- Clean Architecture, DDD  
- CQRS with **MediatR**  
- Repository Pattern  
- Result Pattern for unified API responses  
- Design Patterns (Strategy, Factory, Decorator, etc.)  

###  Security
- JWT Authentication, Refresh Tokens, OAuth2  
- Role & Policy-Based Authorization  
- Azure Key Vault integration  

###  Background Jobs
- **Hangfire** for scheduled/background tasks  

###  Caching & Performance
- **Hybrid Caching** (In-Memory + Redis)  
- Redis distributed cache  
- In-process caching for high-frequency queries  
- API Rate Limiting  

###  Logging & Monitoring
- **Serilog** with **Seq** for structured logging  


###  Validation & Configuration
- **FluentValidation** for input models  
- Strongly-typed configuration classes  
- Environment-based configuration (`appsettings.*.json`)  



###  API Management
- **OpenAPI/Swagger** for documentation  
- API Versioning and Health Checks  

###  DevOps
- Docker & Docker Compose  
- Deployment scripts for cloud and container hosts  

###  Error Handling
- Centralized Exception Middleware  
- RFC 7807 Problem Details responses  

###  Source Control
- Git & GitHub for collaboration  

---

## 🏗 Architecture
SurveyBasket follows **Clean Architecture**, promoting separation of concerns:
