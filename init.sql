-- Crear la base de datos
CREATE DATABASE LabSQLi;
GO

USE LabSQLi;
GO

-- Crear tabla de usuarios
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL
);
GO

-- Insertar datos de prueba
INSERT INTO Usuarios (NombreUsuario, Password) VALUES ('admin', 'admin123');
INSERT INTO Usuarios (NombreUsuario, Password) VALUES ('user1', 'user123');
INSERT INTO Usuarios (NombreUsuario, Password) VALUES ('bob', 'password111');
GO
