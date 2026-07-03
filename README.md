# TrycoreTest - FrontEnd

Este proyecto contiene la aplicación FrontEnd desarrollada con **Angular 19** como parte de la prueba técnica. A continuación, se detallan las instrucciones necesarias para configurar, instalar y ejecutar el proyecto en un entorno local.

---

## 🚀 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado lo siguiente en tu máquina:
* [Node.js](https://nodejs.org/) (Versión 18.x o superior recomendada)
* [Angular CLI](https://angular.dev/tools/cli) (Versión 19.x)
* Un gestor de paquetes como `npm` (incluido por defecto con Node.js)

---

## 🛠️ Instalación y Configuración

Sigue estos pasos para poner en marcha el proyecto localmente:

### 1. Navegar al directorio del FrontEnd
Abre tu terminal en la raíz del repositorio y muévete a la carpeta correspondiente al proyecto de Angular:

cd FrontEnd

### 2. Instalar las dependencias
Ejecuta el siguiente comando para descargar e instalar todas las librerías necesarias especificadas en el package.json:

Bash
npm install

### 3. Para compilar la aplicación y levantar el servidor de desarrollo local, ejecuta:

ng serve
Una vez que el proceso termine con éxito, abre tu navegador web e ingresa a la siguiente dirección:
👉 http://localhost:4200

La aplicación se recargará automáticamente si realizas cambios en cualquiera de los archivos fuente.

---

### 🏗️ Comandos Útiles
Si deseas realizar otras acciones comunes en Angular, puedes usar:

Compilación para producción: ng build (Genera los archivos listos para despliegue en la carpeta dist/).

Ejecutar pruebas unitarias: ng test (Para suites de pruebas automatizadas).

🌿 Flujo de Trabajo (Git Flow)
Este repositorio se ha gestionado siguiendo las mejores prácticas de control de versiones solicitadas:

main: Código estable y listo para producción.

develop: Rama de integración para el desarrollo.

feature/*: Ramas específicas por cada funcionalidad.

release/*: Preparación y estabilización previa a la entrega final.