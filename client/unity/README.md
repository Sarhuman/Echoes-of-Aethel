# Cliente Unity — prova técnica U0

Abra esta pasta como projeto no Unity Hub com **Unity 6.6 `6000.6.6f1`**. `Assets/Scenes/U0_Inspection.unity` contém apenas chão, marcador de escala, câmera, luz e diagnóstico de hardware. Não há personagem, combate, mapa de campanha ou controles.

O projeto e os manifests foram criados pelo editor instalado em 16/10/2026. Ele usa o renderer padrão da criação mínima; **URP ainda não está instalado**. O build de inspeção pode ser reproduzido no Windows, com o módulo Windows Build Support, executando:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe' -batchmode -nographics -quit -projectPath '<caminho-absoluto>\client\unity' -executeMethod U0Inspection.CreateSceneAndBuild -logFile '<caminho-absoluto>\unity-u0-build.log'
```

O executável sai em `client/unity/Build/EchoesOfAethel-U0.exe`, diretório ignorado pelo Git. Em 16/10/2026, a compilação terminou com `U0 Windows inspection build succeeded` e o executável iniciou fora do editor em modo `-batchmode -nographics`. O log registrou Unity `6000.6.6f1`, CPU Xeon E5-2630 v3, 16.224 MB de RAM e resolução 1424×720. A GPU apareceu como `Null Device` porque o teste desabilitou a renderização; isso **não valida** a RX 580, qualidade visual, FPS, tempo de frame nem memória de vídeo real. A abertura visual e a medição no PC continuam pendentes.

`shared/csharp/` ainda não foi integrado às assemblies Unity. Os clientes Godot e Unreal permanecem preservados. O gate C0 de conteúdo segue aberto e esta cena não antecipa gameplay.

[Checagem de portabilidade do núcleo C#](../../docs/engineering/unity-domain-portability.md): o código de produção compilou para `netstandard2.1` fora da Unity; importação, persistência e player ainda precisam de testes na engine.
