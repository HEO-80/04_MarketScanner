# 📊 Market Scanner - Vigilancia de Uniswap V3 (C#)

Este repositorio contiene una aplicación de consola en C# diseñada para monitorizar el estado de un pool de liquidez en Uniswap V3 en tiempo real, utilizando una conexión a un nodo de desarrollo local.

## 🛠️ Especificaciones Técnicas

El programa utiliza la biblioteca `Nethereum` para interactuar con la blockchain mediante llamadas RPC de solo lectura (*calls*), sin necesidad de firmar transacciones ni gastar gas.

El flujo de ejecución (`Program.cs`) se compone de las siguientes características técnicas:
1.  **Conexión de Red:** Establece conexión con un cliente RPC local en `http://127.0.0.1:8545` (típicamente Anvil o Hardhat).
2.  **Consulta de Contratos Inteligentes:** Utiliza el espacio de nombres `Nethereum.Contracts` para estructurar llamadas de lectura a contratos ya desplegados en la red.
3.  **Lectura de Estado (Slot0):** Instancia la función `slot0`, específica de la arquitectura de contratos de Uniswap V3, apuntando a la dirección del pool principal de DAI/WETH en la red principal (Mainnet). La función `slot0` devuelve los datos fundamentales del pool, incluyendo el precio actual y el *tick* de liquidez.
4.  **Bucle de Monitoreo:** Ejecuta la lectura de datos dentro de un ciclo infinito (`while (true)`), utilizando un bloque `try/catch` para el manejo de errores y actualizando la información cada 3 segundos de manera continua en la consola.

---

## 📡 Arquitectura Conceptual (Cómo entenderlo)

Para comprender mejor el propósito de este módulo dentro del ecosistema de arbitraje, podemos compararlo con el sistema de radar de un barco:

* **El Radar (Este escáner):** Su trabajo es emitir una señal continua (el escaneo cada 3 segundos) hacia un objetivo específico (el Pool de Uniswap V3) para obtener sus coordenadas actuales (el precio de los tokens). No toma decisiones, no dispara operaciones de compra y no gasta combustible (gas). Simplemente observa el mercado y extrae la información matemática cruda. En versiones posteriores del bot, esta es la "vista" que utilizará el "Cerebro" para detectar desequilibrios de precios y decidir cuándo es rentable atacar.

## 🚀 Configuración y Ejecución

Al ser un script puramente de lectura que apunta a datos públicos en un nodo local, no requiere claves privadas ni la configuración de un archivo `.env`.

1.  Asegúrate de tener un nodo local (Anvil) ejecutándose en el puerto `8545`. Para que esto funcione correctamente y encuentre la dirección del pool real, el nodo local debe estar ejecutando un *fork* (bifurcación) de la red principal de Ethereum.
2.  Abre la terminal en la carpeta raíz de este proyecto y ejecuta:
    ```bash
    dotnet run
    ```

---
---

# 📊 Market Scanner - Uniswap V3 Watcher (C#) [EN]

This repository contains a C# console application designed to monitor the state of a Uniswap V3 liquidity pool in real-time, using a connection to a local development node.

## 🛠️ Technical Specifications

The program utilizes the `Nethereum` library to interact with the blockchain through read-only RPC calls, without the need to sign transactions or spend gas.

The execution flow (`Program.cs`) consists of the following technical features:
1.  **Network Connection:** Establishes a connection with a local RPC client at `http://127.0.0.1:8545` (typically Anvil or Hardhat).
2.  **Smart Contract Querying:** Uses the `Nethereum.Contracts` namespace to structure read calls to deployed contracts on the network.
3.  **State Reading (Slot0):** Instantiates the `slot0` function, specific to the Uniswap V3 contract architecture, targeting the main DAI/WETH pool address on the Mainnet. The `slot0` function returns fundamental pool data, including the current price and liquidity tick.
4.  **Monitoring Loop:** Executes the data reading within an infinite loop (`while (true)`), using a `try/catch` block for error handling and continuously updating the console information every 3 seconds.

---

## 📡 Conceptual Architecture (How to understand it)

To better understand the purpose of this module within the arbitrage ecosystem, we can compare it to a ship's radar system:

* **The Radar (This scanner):** Its job is to emit a continuous signal (scanning every 3 seconds) towards a specific target (the Uniswap V3 Pool) to get its current coordinates (token prices). It makes no decisions, does not trigger buy trades, and spends no fuel (gas). It simply watches the market and extracts the raw mathematical data. In later versions of the bot, this is the "eyesight" the "Brain" will use to spot price imbalances and decide when it is profitable to strike.

## 🚀 Setup & Execution

Since this is purely a read-only script targeting public data on a local node, it requires no private keys or `.env` file configuration.

1.  Ensure you have a local node (Anvil) running on port `8545`. For this to work properly and find the real pool address, the local node must be running a fork of the Ethereum Mainnet.
2.  Open the terminal in the root folder of this project and run:
    ```bash
    dotnet run
    ```