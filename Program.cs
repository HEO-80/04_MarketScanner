using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts; // <--- ¡ESTA ERA LA LÍNEA QUE FALTABA!

class Program
{
    // 1. Conexión a tu Anvil Local
    static string rpcUrl = "http://127.0.0.1:8545";

    // 2. Dirección del Pool DAI/WETH (Fee 0.3%) en Mainnet
    static string poolAddress = "0xC2e9F25Be6257c210d7Adf0D4Cd6E3E881ba25f8";

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine("   🦅 MARKET SCANNER - VIGILANDO UNISWAP V3     ");
        Console.WriteLine("------------------------------------------------");
        Console.ResetColor();

        var web3 = new Web3(rpcUrl);

        var slot0Function = new Slot0Function();
        // Ahora sí funcionará 'FromAddress' porque hemos importado Nethereum.Contracts
        slot0Function.FromAddress = "0x0000000000000000000000000000000000000000"; 

        Console.WriteLine($"🔭 Conectado a Pool: {poolAddress}");
        Console.WriteLine("⏱️  Escaneando cada 3 segundos...\n");

        while (true)
        {
            try
            {
                var resultado = await web3.Eth.GetContractQueryHandler<Slot0Function>()
                    .QueryDeserializingToObjectAsync<Slot0OutputDTO>(slot0Function, poolAddress);

                var sqrtPriceX96 = resultado.SqrtPriceX96;

                // --- MATEMÁTICAS DE PRECIO ---
                double precioRaw = (double)sqrtPriceX96;
                double dosElevado96 = Math.Pow(2, 96);
                double precioFinal = Math.Pow(precioRaw / dosElevado96, 2);
                
                // Invertimos para ver DAI por ETH
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

            await Task.Delay(3000);
        }
    }
}

// --- CLASES TÉCNICAS (DTOs) ---

[Function("slot0", typeof(Slot0OutputDTO))]
public class Slot0Function : FunctionMessage // Esto ahora funcionará correctamente
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