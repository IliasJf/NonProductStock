create database NonProductManager
use NonProductManager

create table Users
(
idUser int primary key identity(1,1),
username varchar(200) not null,
password varchar(200) not null
)

create table Compon
(
codeComponents varchar(200) primary key,
SupplierName varchar(200),
UnitPrice decimal,
Type varchar(200),
MeasureUnit varchar(200),
EmgStock int,
PoStatus varchar(200),
Status varchar(200),
Description varchar(200),
Commentary varchar(200)
)

create table Opera
(
idOperation int primary key identity(1,1),
codeComponents varchar(200) foreign key references Compon(codeComponents),
quantityIN int,
quantityOUT int,
currentStock int,
mp varchar(200),
recipiendID varchar(200),
site varchar(50),
date datetime,
operationType varchar(50)
)

create table Sto
(
codeComponents varchar(200) primary key,
sumOfQuantityIN int,
sumOfQuantityOUT int,
stock int,
uniteMeasure varchar(50),
Description varchar(200)
)
create table Short
(
codeComponents varchar(200) primary key,
sumOfQuantityIN int,
sumOfQuantityOUT int,
stock int,
emgStock int
)

