# WIN Student Goal Tracker


The **WIN Student Goal Tracker** is a web-based platform that helps
teachers and program staff manage students, track goals, define
benchmarks, and record progress events.

This project was developed as part of the **Computing for Good (C4G)**
initiative.

------------------------------------------------------------------------

# Peer Reviewers

Check out the instructions and [survey here](https://docs.google.com/forms/d/e/1FAIpQLSc53xGhgQvq-ory-oU7sQ8M09nl_jJfPLh-1ABgjcfLxKNDHQ/viewform?usp=publish-editor)


------------------------------------------------------------------------

# Live Prototype

Access the deployed prototype:

https://win.opelly.me

Demo credentials:

Email: opelly@gmail.com\
Password: 1234

------------------------------------------------------------------------

# Key Features

## User Authentication

-   Secure login system
-   JWT authentication with refresh tokens
-   Program-scoped authorization

## Student Management

-   View assigned students
-   Add new students
-   Track graduation dates

## Goal Management

-   Create and manage goals
-   Associate goals with individual students

## Benchmarks

-   Define milestones within a goal
-   Break goals into measurable steps

## Progress Events

-   Record achievements and activities
-   Maintain historical progress logs

------------------------------------------------------------------------

# System Architecture

User Browser\
↓\
Angular Frontend (Web Client)\
↓\
Reverse Proxy (SSL + Routing)\
↓\
.NET Core API (Business Logic)\
↓\
MySQL Database

------------------------------------------------------------------------

# Technology Stack

## Frontend

Angular 20

Responsibilities: - UI rendering - API communication - Form validation -
Responsive design

## Backend

.NET Core 9.0 (C#) with **Dapper ORM**

Responsibilities: - Business logic - Goal and benchmark management -
Student progress tracking - REST API endpoints

## Authentication

JWT Authentication with Refresh Tokens

Features: - Token-based authentication - Refresh token lifecycle -
Program-scoped authorization

## Database

MySQL relational database storing:

-   Users
-   Programs
-   Students
-   Goals
-   Benchmarks
-   Progress Events

------------------------------------------------------------------------

# Infrastructure

The system runs in **four Docker containers** on a VPS.

  Container       Purpose
  --------------- ---------------------
  Angular         Web frontend
  .NET Core API   Business logic
  MySQL           Database
  Traefik         Reverse proxy + SSL

------------------------------------------------------------------------

# User Guide

## Login

Navigate to:

https://win.opelly.me/login

Enter email and password and click **Sign in**.

## Select Program

Choose:

WIN Program -- Teacher (Primary)

## Student Dashboard

The dashboard displays all assigned students with:

-   Student identifier
-   Graduation date
-   Number of goals
-   Number of progress events
-   Last activity date

## Add Student

1.  Click **+ Add a Student**
2.  Enter identifier
3.  Enter expected graduation date
4.  Click **Add Student**

## Goals

Each student can have multiple goals that track academic or program
milestones.

## Benchmarks

Benchmarks break goals into measurable steps.

Example:

Complete Computing for Good

## Progress Events

Events document activities related to a goal.

Example:

Took C4G -- loved it!

------------------------------------------------------------------------

# Project Structure

WinStudentGoalTracker ├── frontend ├── backend ├── database ├── docker
└── README.md

------------------------------------------------------------------------

# Contributors

| Name         | Role                           |
|--------------|--------------------------------|
| Raul Rosado  | Infrastructure & Governance    |
| Armin Abaye  | UX/UI & Product Strategy       |
| Ivan Pelly   | Full Stack Development         |
| Oliver Pelly | Backend & Security             |
| Vraj Patel   | Front-End, Integration & AI    |

------------------------------------------------------------------------

# Partner Organization

WIN Program

The WIN Student Goal Tracker supports teachers in tracking student
goals, benchmarks, and progress events.

------------------------------------------------------------------------

# License

Educational and research use.
