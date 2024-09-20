# mos-order-history-added-delete-order-func

Microservice Order System - Order History Microservice

[See Wiki for details about the Order History Added Delete Order Function](https://github.com/HammerheadShark666/mos-order-history-added-delete-order-func/wiki))  

This project is an **Azure Function** designed to delete order records. It is built using **.NET 8** and interacts with an **SQL Server** database for storage. The function listens for messages from **Azure Service Bus** to trigger the deletion of order records. It is set up with a **CI/CD pipeline** for seamless deployment.

## Features

- **Order Deletion**: Listens for messages from Azure Service Bus to delete specified order records from the SQL Server database.
- **SQL Server Database**: Stores order information and allows for deletion based on order ID.
- **Azure Service Bus Integration**: Consumes messages from the service bus to trigger deletions of order records.
- **Scalable Serverless Architecture**: Utilizes Azure Functions for on-demand execution and scaling.
- **CI/CD Pipeline**: Automated build and deployment using **GitHub Actions**.

---

## Technologies Used

- **.NET 8**
- **C#**
- **Azure Functions**
- **SQL Server** (Azure SQL)
- **Azure Service Bus**
- **GitHub Actions** for CI/CD

---
