# HUD, controles e navegação — direção para a prova 3D

**Estado:** proposta de implementação gradual, 14/10/2026. Os dois textos recebidos sobre HUD/Quest Log e NavMesh são material de referência, não código pronto nem emenda automática aos controles aprovados. A versão jogável continua em Godot; `client/unreal/` contém apenas o projeto e módulo mínimos, sem widget ou mapa.

## HUD próprio de Aethel

Adotar a **hierarquia** proposta: no canto superior esquerdo, retrato/insígnia e nível, Vida longa em vermelho, Éter azul abaixo, Fôlego em linha fina. Moldura de metal e pedra, recortes rúnicos, contraste alto e números atuais/máximos; não reproduzir bordas, fontes ou composição específica de Castlevania. A barra de boss continua independente e fora dos telegraphs. Nível não substitui valor numérico de HP/MP. Permitir escala de UI, zona segura da tela, textos legíveis e efeitos reduzidos. A arte final depende de mockup, teste em 720p/1080p e navegação com controle.

O exemplo `UAethelCastlevaniaHUD` acerta ao tratar UMG como apresentação, mas precisa de nomes próprios, widget asset real e validação de máximos antes de dividir (máximo zero produz valor inválido). Dados chegam de estado confirmado; o widget não calcula dano, custo, regeneração ou recompensa. Atualizar em mudança relevante, não em todo `Tick`. O estilo pode ser prototipado no Godot sem definir automaticamente a arte final de Unreal.

## Diário sem economia dentro do widget

`J` continua abrindo o diário. Duas seções: **Campanha** e **Secundárias**; cada uma mostra fatos observados, objetivo, local, dificuldade sinalizada e recompensa conhecida. O [contrato das dungeons opcionais](../engineering/optional-dungeon-contract.md) define os estados e retorno. O componente C++ colado na referência é insuficiente: uma lista `EditAnywhere` mutada por `AcceptQuest`/`CompleteQuest` não valida gates, não persiste save, não distingue vitória de resgate de prêmio e pode apagar ou duplicar progresso. O serviço de campanha valida e grava; o componente/UI projeta o resultado e avisa a tela após commit. `FText` localizado é preferível a texto fixo de UI em `FString`.

## Input e click-to-move

Manter **WASD/setas, C, J, I, 1–5, Q/E/R/F, espaço** da versão aprovada. Mouse esquerdo já ataca e direito ativa Impacto no Godot; não trocar esse contrato silenciosamente por Diablo II. No cliente Unreal, **clique para mover é opção futura remapeável**, testada em uma arena 3D antes de decidir se convive com WASD. Se adotada, um clique em chão define destino uma vez, clique em inimigo envia ataque somente com alvo/alcance válidos, e input de combate cancela ou prioriza a rota conforme regra clara. Segurar o mouse não recalcula `SimpleMoveToLocation` a cada frame. Shift para atacar parado e alternância de habilidade no botão direito são hipóteses de playtest, não controles atuais. Não reservar F1–F8 para essa hipótese enquanto atalhos e acessibilidade não forem revistos.

## Navegação de mapa e dungeons

Os 12 mapas principais e a primeira dungeon usam blockout **autoral**, obstáculos e `NavMeshBoundsVolume` dimensionados no editor; testar paredes, poços, degraus, atalhos, largura de cápsulas e rota de retirada. A navegação dinâmica fica para o momento em que peças mudarem em runtime. A [documentação da Epic sobre NavMesh](https://dev.epicgames.com/documentation/unreal-engine/navigation-system-in-unreal-engine?lang=en-US) distingue modos Static, Dynamic e Dynamic Modifiers Only; [exemplos de geração dinâmica](https://dev.epicgames.com/documentation/unreal-engine/overview-of-how-to-modify-the-navigation-mesh-in-unreal-engine) alertam para custo computacional. Logo, `Runtime Generation = Dynamic` não é pré-requisito de todo projeto nem garantia de navegação correta.

O gerador colado nos anexos **não** prova funcionamento: a caixa total soma apenas centros das salas, não seus limites; posicionar o ator novo no socket antigo ignora deslocamento do socket da peça; `GetOverlappingActors` em `TileBounds` com `NoCollision` não comprova separação; o código não garante boss/saída conectados, seed persistente, pontos transitáveis ou build em tempo aceitável. `deltaTime` não corresponde ao parâmetro `DeltaTime`, `print` não é log C++ da Unreal e recalcular destino a cada `PlayerTick` pode reiniciar o caminho continuamente. Não copiar `SetActorScale3D(Extents / 100.0f)` como regra universal de tamanho do volume.

Para uma dungeon modular posterior: peças com conectores tipados e bounds reais; montagem com seed salva; verificação de sobreposição antes de confirmar; grafo conectado de entrada, objetivo, boss e saída; NavMesh cobrindo toda superfície; teste de caminho para herói e inimigo; fallback autoral se a geração falhar. Nunca publicar um layout sem rota de saída. O minimapa translúcido vem **depois** que navegação e legibilidade do cenário estiverem aprovadas; deve mostrar apenas espaço descoberto e não revelar boss ou saída secretos.

## Cadência de um ano, com gates verificáveis

O prazo de um ano orienta a priorização, não é promessa de entrega dos 12 mapas. Primeiro: projeto Unreal abre/compila, Bastião anda em um blockout, HUD lê estado real. Depois: combate e retorno Cidadela–Bosque com save/quest/loot e NPCs. Em seguida: uma dungeon opcional autoral, diário e recompensa. Só após uma rota estável: variação modular, outras classes e expansão dos atos. Cada etapa termina com build Windows, playtest, métricas de desempenho e registro de defeitos antes de aumentar conteúdo. Protótipos visuais Godot podem informar o desenho, mas não substituem teste do cliente Unreal.
