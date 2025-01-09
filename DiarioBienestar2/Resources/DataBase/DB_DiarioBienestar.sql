-- Crear la base de datos si no existe
CREATE DATABASE IF NOT EXISTS DiarioBienestar;

-- Usar la base de datos creada
USE DiarioBienestar;

-- Crear la tabla Registro si no existe
CREATE TABLE IF NOT EXISTS Registro (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    nombre_usuario VARCHAR(30) NOT NULL,
    contrasena VARCHAR(60) NOT NULL
);

-- Crear la tabla Registro_Diario si no existe
CREATE TABLE IF NOT EXISTS Registro_Diario (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    id_usuario INT NOT NULL,
    registro VARCHAR(500),
    intensidad INT NOT NULL,
    energia INT NOT NULL,
    fecha DATE NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES Registro(ID) ON DELETE CASCADE
);

-- Ver todos los registros de la tabla Registro_Diario
SELECT * FROM Registro_Diario;

-- Ver todos los registros de la tabla Registro
SELECT * FROM Registro;

-- Consultar la intensidad y energía de los registros de los últimos 7 días
SELECT intensidad, energia FROM Registro_Diario WHERE fecha >= CURDATE() - INTERVAL 7 DAY;
SELECT registro, intensidad, energia, fecha FROM Registro_Diario;

