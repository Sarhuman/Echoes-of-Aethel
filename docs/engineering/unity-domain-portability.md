# Portabilidade do núcleo C# para Unity 6.6

Verificação de 16/10/2026. O cliente principal é `client/unity/`; o projeto criado localmente em `client/unity/Echoes-of-Aethel/` não faz parte do repositório nem substitui o cliente principal.

## Evidência executada

`dotnet build tools/portability/Aethel.UnityApiSurface.csproj --configuration Release` compilou **todos os arquivos de produção** em `Aethel.WorldMemory` e `Aethel.Campaign` para `netstandard2.1`, com zero avisos e zero erros. O projeto usa C# 7.2 e não compila os testes como código de produção. Essa é uma checagem do conjunto de APIs e da sintaxe; não equivale a importar o núcleo na Unity, executar save no player ou validar IL2CPP.

O projeto Unity 6.6 gerado e o build U0 já compilaram, mas **sem referência ao núcleo C#**. O editor aberto na máquina reconheceu a AMD Radeon RX 580 2048SP em Direct3D 11; a prova U0 em `-nographics` registrou `Null Device`. Nenhum FPS foi medido.

## Integração proposta

Manter as fontes de domínio em `shared/csharp/`, como autoridade única. Empacotar `Aethel.WorldMemory` e `Aethel.Campaign` como pacotes Unity locais com `package.json` e assemblies separadas, referenciados por caminhos relativos no manifesto do projeto. Assim, a Unity lê as mesmas fontes que o harness de regressão, sem cópias divergentes. [A documentação da Unity permite pacotes locais fora de `Assets` e caminhos relativos ao diretório `Packages`](https://docs.unity3d.com/cn/6000.0/Manual/upm-ui-local.html); [scripts em pacotes devem ter assembly definitions próprias](https://docs.unity3d.com/ru/2019.4/Manual/cus-asmdef.html).

Antes de ligar o manifesto, verificar no editor principal que as duas assemblies compilam sem erro, que o `DataContractJsonSerializer`, `File.Replace` e `FileStream.Flush(true)` funcionam em um teste EditMode com pasta temporária, e que nenhuma escrita usa saves Godot/Unreal. Unity usa `.NET Standard 2.1` como perfil padrão, mas a compatibilidade real do player ainda exige teste no Windows. [Referência do perfil Unity](https://docs.unity3d.com/kr/6000.0/Manual/dotnet-profile-support.html).

As regras de campanha não precisam de `MonoBehaviour`. Física, ator, fila persistente de dano e apresentação seguem sem adaptador Unity; não chamar o núcleo compilado de gameplay integrado. O gate C0 permanece aberto.
