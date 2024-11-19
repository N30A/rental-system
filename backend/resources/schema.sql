USE master;

IF DB_ID('RentalSystemDB') IS NULL
BEGIN
	CREATE DATABASE RentalSystemDB;
END

GO
USE RentalSystemDB;

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Address')
BEGIN
	CREATE TABLE Address (
		AddressID INT IDENTITY PRIMARY KEY,
		Street NVARCHAR(100) NOT NULL,
		PostalCode VARCHAR(10) NOT NULL,
		City NVARCHAR(100) NOT NULL,
		Country NVARCHAR(50) NOT NULL,

		CONSTRAINT UniqueAddress UNIQUE (Street, PostalCode, City, Country)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Premise')
BEGIN
	CREATE TABLE Premise (
		PremiseID INT PRIMARY KEY,
		Name VARCHAR(50) NOT NULL UNIQUE,
		AddressID INT NOT NULL UNIQUE,

		FOREIGN KEY (AddressID) REFERENCES Address(AddressID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Category')
BEGIN
	CREATE TABLE Category (
		CategoryID INT PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL,
		ParentCategoryID INT,

		FOREIGN KEY (ParentCategoryID) REFERENCES Category(CategoryID),

		CONSTRAINT UniqueCategory UNIQUE (Name, ParentCategoryID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Item')
BEGIN
	CREATE TABLE Item (
		ItemID INT IDENTITY PRIMARY KEY,
		Name NVARCHAR(50) NOT NULL UNIQUE,
		Description NVARCHAR(500),
		Comment NVARCHAR(500),
		Price MONEY NOT NULL CHECK (Price >= 0),
		CategoryID INT,

		FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'RentalItem')
BEGIN
	CREATE TABLE RentalItem (
		ItemID INT PRIMARY KEY,
		ReplacementCost MONEY NOT NULL CHECK (ReplacementCost >= 0),

		FOREIGN KEY (ItemID) REFERENCES Item(ItemID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'SingleUseItem')
BEGIN
	CREATE TABLE SingleUseItem (
		ItemID INT PRIMARY KEY,

		FOREIGN KEY (ItemID) REFERENCES Item(ItemID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Customer')
BEGIN
	CREATE TABLE Customer (
		CustomerID INT IDENTITY PRIMARY KEY,
		PhoneNumber NVARCHAR(20) NOT NULL UNIQUE,
		Email NVARCHAR(254) NOT NULL UNIQUE,
		AddressID INT NOT NULL,

		FOREIGN KEY (AddressID) REFERENCES Address(AddressID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Individual')
BEGIN
	CREATE TABLE Individual (
		CustomerID INT PRIMARY KEY,
		FirstName NVARCHAR(50) NOT NULL,
		LastName NVARCHAR(100) NOT NULL,
		NationalIDNumber NVARCHAR(50) NOT NULL UNIQUE,

		FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'Business')
BEGIN
	CREATE TABLE Business (
		CustomerID INT PRIMARY KEY,
		Name NVARCHAR(100) NOT NULL,
		OrganisationNumber NVARCHAR(50) UNIQUE,

		FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'OrderStatus')
BEGIN
	CREATE TABLE OrderStatus (
		OrderStatusID INT IDENTITY PRIMARY KEY,
		Name VARCHAR(25) NOT NULL UNIQUE
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'DeliveryMethod')
BEGIN
	CREATE TABLE DeliveryMethod (
		DeliveryMethodID INT IDENTITY PRIMARY KEY,
		Name VARCHAR(25) NOT NULL UNIQUE
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'ReturnMethod')
BEGIN
	CREATE TABLE ReturnMethod (
		ReturnMethodID INT IDENTITY PRIMARY KEY,
		Name VARCHAR(25) NOT NULL UNIQUE
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'PaymentMethod')
BEGIN
	CREATE TABLE PaymentMethod (
		PaymentMethodID INT IDENTITY PRIMARY KEY,
		Name VARCHAR(25) NOT NULL UNIQUE
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'CustomerOrder')
BEGIN
	CREATE TABLE CustomerOrder (
		CustomerOrderID INT IDENTITY PRIMARY KEY,
		DeliveryDate DATE NOT NULL CHECK (DeliveryDate >= CONVERT(DATE, GETDATE())),
		ReturnDate Date NOT NULL CHECK (ReturnDate >= CONVERT(DATE, GETDATE())),
		CustomerID INT NOT NULL,
		DeliveryAddressID INT NOT NULL,
		PickupPremiseID INT NOT NULL,
		ReturnPremiseID INT NOT NULL,
		DeliveryMethodID INT NOT NULL,
		ReturnMethodID INT NOT NULL,
		PaymentMethodID INT NOT NULL,
		OrderStatusID INT NOT NULL,

		FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
		FOREIGN KEY (DeliveryAddressID) REFERENCES Address(AddressID),
		FOREIGN KEY (PickupPremiseID) REFERENCES Premise(PremiseID),
		FOREIGN KEY (ReturnPremiseID) REFERENCES Premise(PremiseID),
		FOREIGN KEY (DeliveryMethodID) REFERENCES DeliveryMethod(DeliveryMethodID),
		FOREIGN KEY (ReturnMethodID) REFERENCES ReturnMethod(ReturnMethodID),
		FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethod(PaymentMethodID),
		FOREIGN KEY (OrderStatusID) REFERENCES OrderStatus(OrderStatusID),

		CONSTRAINT UniqueOrder UNIQUE (
			DeliveryDate,
			ReturnDate,
			CustomerID,
			DeliveryAddressID,
			PickupPremiseID,
			ReturnPremiseID,
			DeliveryMethodID,
			ReturnMethodID,
			PaymentMethodID,
			OrderStatusID
		)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'CustomerOrderItem')
BEGIN
	CREATE TABLE CustomerOrderItem (
		CustomerOrderID INT,
		ItemID INT,
		Quantity INT NOT NULL CHECK (Quantity > 0),

		PRIMARY KEY (CustomerOrderID, ItemID),
		FOREIGN KEY (CustomerOrderID) REFERENCES CustomerOrder(CustomerOrderID),
		FOREIGN KEY (ItemID) REFERENCES Item(ItemID)
	);
END

IF NOT EXISTS (SELECT name FROM sys.tables WHERE name = 'ItemInventory')
BEGIN
	CREATE TABLE ItemInventory (
		ItemID INT,
		PremiseID INT,
		Quantity INT NOT NULL CHECK (Quantity >= 0),
		InventoryDate DATE,

		PRIMARY KEY (ItemID, PremiseID),
		FOREIGN KEY (ItemID) REFERENCES Item(ItemID),
		FOREIGN KEY (PremiseID) REFERENCES Premise(PremiseID)
	);
END