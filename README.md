# HolyBird Resort Management System

HolyBird Resort Management System is a desktop application designed to support room reservation, occupancy management, check-in/check-out operations, and compensation processing for a resort environment.

Developed as part of the Database Management Systems course at the University of Science, VNU-HCM, the project focuses on database design, transaction processing, and concurrency control in multi-user environments.

## Overview

The system enables customers to search and reserve rooms while allowing resort staff to manage room information, customer accounts, reservations, compensation records, and payment processes.

A key objective of the project is ensuring data consistency when multiple users perform concurrent operations on the system. To address this challenge, the application incorporates transaction management mechanisms and analyzes common concurrency issues such as Lost Update and Deadlock scenarios.

## Technology Stack

**Language**

* C#

**Framework**

* .NET Framework 4.8
* Windows Forms (WinForms)

**Database**

* Microsoft SQL Server

**Data Access**

* ADO.NET

**UI Library**

* Guna UI2 WinForms

**Version Control**

* Git
* GitHub

## Core Functionalities

### Customer

* Manage account information
* Search available rooms
* Create and monitor reservations
* View booking history
* Request check-out
* Complete payments using multiple payment methods

### Staff

* Manage rooms and room categories
* Register customer groups and accounts
* Process check-in and check-out requests
* Record compensation and damage reports
* Generate invoices
* Monitor room occupancy status

## Database & Transaction Management

The project was designed around a relational database architecture consisting of customer, reservation, room, transaction, compensation, and account management modules.

Key database concepts applied include:

* Relational database modeling
* Primary and foreign key constraints
* Stored procedures
* Triggers
* Transaction management
* Concurrency control
* Deadlock analysis and prevention

## Team

| Student ID | Full Name            |
| ---------- | -------------------- |
| 23120189   | Hoàng Quốc Việt      |
| 23120193   | Trần Kim Yến         |
| 23120201   | Nguyễn Thị Trúc Hằng |
| 23120209   | Lê Hoàng Nhật Anh    |
| 23120237   | Lê Lâm Trí Đức       |

## My Contributions

**Trần Kim Yến**

* Analyzed transaction conflict scenarios and concurrency issues, demonstrated common problems such as Lost Update and Deadlock, and proposed resolution strategies to ensure data consistency.
* Designed and implemented the relational database schema, including table structures, relationships, constraints, stored procedures, and triggers.
* Developed and maintained the application following a 3-layer architecture (Presentation Layer, Business Logic Layer, Data Access Layer).
* Implemented database connectivity and data processing using ADO.NET with SQL Server.
* Designed user interfaces for customer and staff modules, and participated in system analysis, testing, and validation of core functionalities.

## Setup

### Database

1. Create a SQL Server database named `DB_HolyBird`.
2. Execute the provided SQL script to create tables, constraints, triggers, and stored procedures.

### Application

1. Open `HolyBirdResort.sln` using Visual Studio 2022.
2. Configure the SQL Server connection string.
3. Build and run the application.

## Academic Information

**Course:** Database Management Systems

**Institution:** University of Science, VNU-HCM

**Project Type:** Team Project
