# Portabilidade do núcleo C# para Unity 6.6

Verificação de 16/10/2026. O cliente principal é `client/unity/`; o projeto criado localmente em `client/unity/Echoes-of-Aethel/` não faz parte do repositório nem substitui o cliente principal.

## Evidência executada

`dotnet build tools/portability/Aethel.UnityApiSurface.csproj --configuration Release` compilou **todos os arquivos de produção** em `Aethel.WorldMemory` e `Aethel.Campaign` para `netstandard2.1`, com zero avisos e zero erros. O projeto usa C# 7.2 e não compila os testes como código de produção.

O pacote local `shared/csharp/package.json` foi importado pela Unity 6.6 como `com.sarhuman.aethel.domain`. Uma assembly `Aethel.Domain` agrupa as duas pastas porque há membros `internal` utilizados entre elas. Os testes legados do harness estão numa assembly condicionada a `AETHEL_INCLUDE_LEGACY_TESTS`, desativada no build normal. `U1DomainSmoke.Verify` passou no editor: atributos 172/52, roundtrip JSON, `File.Replace` e `FileStream.Flush(true)`. O build Windows U1 compilou e iniciou em modo sem gráficos com `AETHEL_U1_RUNTIME_SMOKE_OK`. Não houve save de campanha no player nem validação IL2CPP.

O editor aberto na máquina reconheceu a AMD Radeon RX 580 2048SP em Direct3D 11; as provas headless registraram `Null Device`. Nenhum FPS foi medido.

## Integração aplicada

As fontes de domínio ficam em `shared/csharp/`, como autoridade única, e são referenciadas pelo caminho relativo no manifesto `client/unity/Packages/manifest.json`. A Unity lê as mesmas fontes que o harness de regressão, sem cópias divergentes. [A documentação da Unity permite pacotes locais fora de `Assets` e caminhos relativos ao diretório `Packages`](https://docs.unity3d.com/cn/6000.0/Manual/upm-ui-local.html); [scripts em pacotes devem ter assembly definitions próprias](https://docs.unity3d.com/ru/2019.4/Manual/cus-asmdef.html).

O teste EditMode usa um arquivo temporário próprio, sem tocar saves Godot/Unreal. Unity usa `.NET Standard 2.1` como perfil padrão; o teste de inicialização Windows cobriu o player Mono, mas serialização e escrita de save no player ainda precisam de teste. [Referência do perfil Unity](https://docs.unity3d.com/kr/6000.0/Manual/dotnet-profile-support.html).

As regras de campanha não precisam de `MonoBehaviour`. O protótipo U1 usa `CharacterRules` para HP/MP e `CombatMath` para dano; física, loot, quest, save e apresentação definitiva seguem sem adaptador Unity. O gate C0 permanece aberto.
