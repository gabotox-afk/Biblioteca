# Biblioteca_Tasada_Moreno_Reyes

Sistema de gestión de préstamos para una biblioteca municipal, hecho como trabajo
práctico. Permite registrar préstamos, devoluciones, renovaciones y reservas de libros,
controlando los límites y multas de cada tipo de socio, y ofrece reportes de consulta.

Desarrollado en **C#** con **.NET 8.0**, **Entity Framework Core** y base de datos
**SQLite**.

## Integrantes

- Ignacio Tasada
- Gabriel Moreno
- Franco Reyes


## Cómo ejecutarlo

Desde la carpeta del proyecto:

```bash
cd Biblioteca_Tasada_Moreno_Reyes
dotnet run
```

La base de datos `Biblioteca.db` ya viene con datos de prueba (5 libros, 5 socios de
distintos tipos y algunos préstamos activos y vencidos).

Al iniciar, la aplicación muestra la lista de libros y luego un menú interactivo.

## Funcionalidades

### Préstamos y socios
- Tres tipos de socio (Común, Estudiante, Docente) con distintos límites de libros,
  días de préstamo y multa por día de demora.
- **Préstamo**: busca el libro por título o autor, verifica disponibilidad y registra
  el préstamo calculando la fecha de vencimiento según el tipo de socio.
- **Devolución**: registra la fecha de devolución, calcula la multa si hubo demora y
  notifica si el libro tenía una reserva pendiente.
- **Renovación**: extiende una única vez el préstamo, si no está vencido ni reservado
  por otro socio.
- **Reserva**: cuando no hay copias disponibles (o desde el menú), un socio puede
  reservar un libro. No se permite más de una reserva pendiente del mismo libro por
  socio.

### Reportes
1. Libros más prestados (top 5).
2. Socios con multas pendientes y monto total adeudado.
3. Préstamos vencidos.
4. Disponibilidad de un libro (copias disponibles y reservas pendientes).
5. Historial de un socio (préstamos y reservas).
6. Ranking de los 10 socios más activos.

## Reglas de negocio

- Un socio inactivo no puede prestar ni reservar.
- Un socio con multas pendientes no puede retirar nuevos libros.
- No se puede prestar un libro sin copias disponibles (se ofrece reservar).
- Cada socio tiene un límite de libros simultáneos según su tipo.
- La fecha de vencimiento y las multas se calculan automáticamente.
