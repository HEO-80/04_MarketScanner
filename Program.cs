using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;

class Program
{
    // 1. Conexión a tu Anvil Local
    static string rpcUrl = "http://127.0.0.1:8545";

    // 2. Dirección del Pool DAI/WETH (Fee 0.3%) en Mainnet
    // Esta es la "piscina" real que estamos vigilando
    static string poolAddress = "0xC2e9F25Be6257c210d7Adf0D4Cd6E3E881ba25f8";

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine("   🦅 MARKET SCANNER - VIGILANDO UNISWAP V3     ");
        Console.WriteLine("------------------------------------------------");
        Console.ResetColor();

        var web3 = new Web3(rpcUrl);

        // Definimos la función "slot0" del contrato de Uniswap
        // (Es la función que guarda el precio actual y otros datos técnicos)
        var slot0Function = new Slot0Function();
        slot0Function.FromAddress = "0x0000000000000000000000000000000000000000"; // Dirección dummy

        Console.WriteLine($"🔭 Conectado a Pool: {poolAddress}");
        Console.WriteLine("⏱️  Escaneando cada 3 segundos...\n");

        while (true) // Bucle infinito (Vigilancia 24/7)
        {
            try
            {
                // A. Llamamos al contrato para leer el estado actual
                var resultado = await web3.Eth.GetContractQueryHandler<Slot0Function>()
                    .QueryDeserializingToObjectAsync<Slot0OutputDTO>(slot0Function, poolAddress);

                // B. Extraemos el precio "crudo" (Formato matemático complejo de Uniswap)
                var sqrtPriceX96 = resultado.SqrtPriceX96;

                // C. TRADUCCIÓN A LENGUAJE HUMANO
                // La fórmula mágica de Uniswap: Price = (sqrtPrice / 2^96)^2
                // (Simplificado para visualización, no usaremos esto para operar exacto todavía)
                double precioRaw = (double)sqrtPriceX96;
                double dosElevado96 = Math.Pow(2, 96);
                double precioFinal = Math.Pow(precioRaw / dosElevado96, 2);
                
                // Ajuste por decimales (DAI vs WETH) para ver el precio de ETH en DAI
                // Como 1 ETH vale miles de DAI, invertimos o ajustamos según el orden del par.
                // En este Pool: Token0 = DAI, Token1 = WETH.
                // El precio que sale es WETH por cada DAI (0.0003...). 
                // Para ver DAI por ETH, hacemos la inversa (1 / precio).
                double precioEthEnDai = 1 / precioFinal;

                Console.Write($"[{DateTime.Now:HH:mm:ss}] 💰 Precio ETH: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"${precioEthEnDai:F2} DAI");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo precio: {ex.Message}");
            }

            // D. Esperar 3 segundos antes de volver a mirar
            await Task.Delay(3000);
        }
    }
}

// --- CLASES TÉCNICAS PARA NETHEREUM (DTOs) ---
// Estas clases sirven para "traducir" lo que devuelve el contrato a variables de C#

[Function("slot0", typeof(Slot0OutputDTO))]
public class Slot0Function : FunctionMessage
{
}

[FunctionOutput]
public class Slot0OutputDTO : IFunctionOutputDTO
{
    [Parameter("uint160", "sqrtPriceX96", 1)]
    public BigInteger SqrtPriceX96 { get; set; }

    [Parameter("int24", "tick", 2)]
    public int Tick { get; set; }

    [Parameter("uint16", "observationIndex", 3)]
    public ushort ObservationIndex { get; set; }

    [Parameter("uint16", "observationCardinality", 4)]
    public ushort ObservationCardinality { get; set; }

    [Parameter("uint16", "observationCardinalityNext", 5)]
    public ushort ObservationCardinalityNext { get; set; }

    [Parameter("uint8", "feeProtocol", 6)]
    public byte FeeProtocol { get; set; }

    [Parameter("bool", "unlocked", 7)]
    public bool Unlocked { get; set; }
}