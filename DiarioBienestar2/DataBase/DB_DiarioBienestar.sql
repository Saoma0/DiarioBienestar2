-- CREATE DATABASE DiarioBienestar;
USE DiarioBienestar;

/*
CREATE TABLE Registro (
	ID INT PRIMARY KEY AUTO_INCREMENT,
	nombre_usuario VARCHAR (30) not NULL,
	contrasena VARCHAR (60) NOT NULL
);
*/

/*
CREATE TABLE Registro_Diario(
	ID INT PRIMARY KEY AUTO_INCREMENT,
	id_usuario INT NOT NULL ,
	registro VARCHAR (500),
	intensidad int NOT NULL ,
	energia INT NOT NULL ,
	fecha DATE NOT NULL,
	FOREIGN KEY (id_usuario) REFERENCES Registro (ID) ON DELETE CASCADE
);
*/

SELECT * FROM Registro_Diario;

SELECT * FROM registro;

SELECT intensidad, energia FROM Registro_Diario WHERE fecha >= CURDATE() - INTERVAL 7 DAY;



