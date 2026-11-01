CREATE DATABASE IF NOT EXISTS productos_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE productos_db;

CREATE TABLE IF NOT EXISTS productos (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    nombre      VARCHAR(100)  NOT NULL,
    descripcion VARCHAR(255)  NOT NULL DEFAULT '',
    precio      DECIMAL(10,2) NOT NULL,
    stock       INT           NOT NULL DEFAULT 0
);

INSERT INTO productos (nombre, descripcion, precio, stock) VALUES
    ('Teclado mecanico', 'Teclado USB con switches rojos', 899.00, 15),
    ('Mouse inalambrico', 'Mouse de 2.4 GHz con receptor USB', 349.50, 30),
    ('Monitor 24 pulgadas', 'Monitor Full HD con entrada HDMI', 2799.99, 8);
