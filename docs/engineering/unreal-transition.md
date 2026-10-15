# Transição de Echoes of Aethel para 3D

**Decisão de direção, 14/10/2026.** O próximo cliente de produção será um ARPG 3D de câmera elevada na Unreal Engine, começando por uma prova curta Cidadela → Bosque. O cliente Godot 4.6 permanece como referência jogável e de regressão até existir uma substituição testada. Esta decisão substitui apenas as instruções anteriores que tornavam Godot/GDScript e pixel art 16-bit o destino final; não altera história, IDs, fórmulas, escolhas, segurança do hub nem direitos dos assets. O esqueleto `.uproject`/C++ para 5.8 está em `client/unreal/`; ainda não há cena ou build Unreal validada.

## Fontes de verdade

1. Este documento rege **engine, forma de migração e critério de equivalência**. A [ficha da primeira rota](../design/first-route-3d.md) e o [manifesto portátil](first-route-content.json) regem o primeiro cenário 3D.
2. [Decisões de produção](production-decisions.md) e [decisões da primeira fatia](decisions-first-slice.md) continuam regendo regras de jogo, combate, save e segurança; cláusulas explicitamente 2D/Godot são históricas para a nova implementação.
3. [GDD de mapas e quests](../GDD/04_mapas_e_quests.md), [roteiro](../GDD/07_roteiro_e_dialogos.md), [classes](../GDD/01_classes_e_atributos.md) e [loot](../GDD/03_sistema_de_loot.md) continuam regendo conteúdo. Não transportar literalmente pixels, tiles, `Resource`, cenas `.tscn` ou física 2:1 para unidades 3D.
4. O [cliente Godot](../../client/godot/) mostra comportamento já jogável, não é a especificação de toda a campanha. Diferenças entre GDD e runtime são registradas antes de migrar cada sistema.

A [direção de HUD, controles e navegação 3D](../design/hud-input-navigation-3d.md) registra a avaliação dos exemplos recebidos e os gates para click-to-move, NavMesh e minimapa. O [contrato de cinemáticas e áudio](../design/unreal-cinematics-and-audio.md) separa infraestrutura C++ de cenas e assets ainda não criados.

## O que já está pronto como esqueleto de conteúdo

| Elemento | Fonte | Estado transferível |
|---|---|---|
| Quatro atos, doze mapas, ordem de acesso, hub e decisões | GDD 04 | Especificado; somente Cidadela e Bosque são mapas jogáveis |
| Prólogo, conselho, diálogos condicionais, confronto e dois finais | GDD 07 | Roteiro; abertura e primeira jornada ganham runtime Godot no PR #22 |
| Cinco classes e fórmulas | GDD 01 e decisões D1–D8 | Especificadas; Bastião é a única classe jogável |
| Criaturas, subchefes e chefes | GDD 04, GDD 05 e `docs/design/prologue-and-act-one.md` | Elenco e parte dos telegraphs definidos; Lobo, Cultista e Garras têm runtime |
| Inventário, drops, comércio e save | GDD 03, `experimental-values.md`, código Godot | Primeira fatia implementada; resto da economia é proposta, não conteúdo pronto |
| Arte e áudio atuais | `docs/media/runtime-art.md`, `runtime-audio.md` | Referência visual/sonora original; PNG 2D não vira modelo 3D automaticamente |

## Contratos que atravessam a engine

- IDs de classe, zona, NPC, quest, opção, item e recompensa não dependem de Actor, Blueprint, nome de arquivo ou posição. O manifesto usa os IDs do GDD e registra qualquer ID **novo e experimental**. Mudanças em IDs persistidos exigem migração.
- Uma decisão confirmada e uma recompensa concedida são persistidas **uma vez**. Um baú não sorteia novamente ao entrar, morrer ou recarregar. Bolsa cheia envia o item à reserva persistente antes de marcar a coleta; falha de gravação mantém o baú fechado.
- `citadel` é o checkpoint da primeira rota. Casa, porão, Bosque e arena são locais de navegação, não novos checkpoints. A Cidadela não tem dano nem inimigos ativos, mesmo após alterações narrativas.
- Apresentação não concede HP, XP, ouro, loot ou progresso. Interação envia comando; serviço valida e grava; UI, VFX, áudio e cinemática reagem ao resultado confirmado. Pular a cinemática chega ao mesmo estado de campanha que assisti-la.
- Save Godot existente **não** é prometido compatível com Unreal. Antes de importar saves reais, construir um conversor versionado com testes de ida, falha e recuperação, preservando cópia intacta do original. A prova 3D usa save próprio de teste.

## Divergências que não podem virar bugs de portabilidade

| Tema | Texto antigo / runtime atual | Regra para a primeira prova 3D |
|---|---|---|
| Engine e escala | GDD 06 e arte 16-bit falam em Godot, tiles 16×16 e projeção 2:1 | Nova geometria em 3D e câmera elevada; manter paleta/silhuetas, não copiar unidades |
| Recursos do Bastião | Algumas Sprints falam em vigor; decisões D2/D5 e slice usam MP | Manter HP/MP/stamina da primeira fatia; revisão de recursos exige decisão separada |
| Loot excedente | GDD 03 propõe `pending_rewards` por instância; save Godot usa `pending_loot` agregado por item | Na prova, manter concessão única + reserva persistente; conversão para itens instanciados é trabalho posterior |
| Save | Godot usa JSON com `CONTENT_VERSION = world-save-2` e envelope próprio | Não carregar o arquivo diretamente como asset Unreal; mapear campos e testar conversão antes de oferecer importação |
| Garras de Musgo | Referências antigas às vezes dizem boss | É subchefe do mapa 01; Silvano é o primeiro boss de ato |
| Comércio | GDD 03 prevê economia ampla; runtime vende catálogo reduzido na Cidadela | Reproduzir apenas serviços/itens já jogáveis; preços atuais são experimentais |

## Ordem de implementação e aceite

1. **Projeto base:** o módulo inicial associa Unreal 5.8 instalada localmente. Abrir e compilar no editor, criar nível Top Down 3D para Windows e registrar instruções de cozinhar e empacotar antes de chamar a base de validada. Não adicionar packs de terceiros sem licença e origem.
2. **Blockout de Cidadela e Bosque:** câmera 3/4, chão 3D, portão, oficina, casa abandonada, porão, poço, arena, volumes de colisão e navegação. A [ficha](../design/first-route-3d.md) define relações, não coordenadas inventadas. Mostrar limites antes de polir cenário.
3. **Loop de equivalência:** Bastião anda, esquiva, ataca, encontra Lobo/Cultista/Garras, coleta recompensa única, decide `act1.spy_hunt`, retorna ao hub e recarrega sem duplicação. Serviços de Bram, Lyra e Sera permanecem acessíveis.
4. **Cinemática curta:** enquadramentos do prólogo e conselho com direção de cena, música original e texto legível. Sem TTS automático. Dublagem só com áudio produzido e aprovado; opção de pular e controles de volume.
5. **Comparação:** testar no mesmo PC uma rota completa, erros de navegação/colisão, clareza dos ataques, legibilidade, tempo de carregamento e desempenho. Só encerrar o cliente Godot quando a build Unreal reproduzir o loop e preservar as decisões de domínio.

## Esqueleto previsto do cliente Unreal

O `.uproject`, os dois targets, o módulo C++ mínimo e as classes de apresentação para cinemática/áudio já existem em `client/unreal/`; **Content, níveis, assets e sistemas de jogo listados abaixo ainda não existem**. O manifesto JSON permanece em `docs/engineering/` como referência portável, não como dado de save recebido sem validação.

```text
client/unreal/EchoesOfAethel.uproject
client/unreal/Content/Aethel/
  Maps/             Cidadela, Bosque, interiores e arenas (blockout antes da arte)
  Characters/       Bastião e NPCs; malhas, rigs e animações
  Enemies/          Lobo, Cultista, Garras; dados e apresentação de ataques
  Items/            definições visuais, ícones e recipientes
  Quests/           falas e condições com IDs do domínio
  Cinematics/       sequências do prólogo e conselho
  UI/               HUD, inventário, comércio e opções
  Audio/            música, efeitos, mixagem e futuras vozes gravadas
client/unreal/Source/EchoesOfAethel/
  Domain/           cálculo, IDs, validação e estados sem dependência de mapa
  Application/      comandos, save, quest, loot e comércio
  Presentation/     ponte para câmera, animação, HUD e áudio
```

Blueprints e níveis montam atores e apresentação; a camada de aplicação confirma efeitos econômicos antes de emitir feedback. O save deve ter versão e fronteira de validação próprias. Antes de anunciar o novo projeto como executável, compilar e abrir o `.uproject` com a instalação 5.8, verificar os módulos e registrar a saída real. O arquivo de projeto isolado não comprova runtime.

## Limites do primeiro marco

Não prometer os 12 mapas em 3D, cinco classes funcionais, dublagem, interiores para cada casa, geração procedural ou save antigo convertido. O primeiro marco entrega **uma rota pequena, jogável e mensurável**. O [GDD 04](../GDD/04_mapas_e_quests.md) permanece a fila de expansão; cada mapa futuro receberá uma ficha como a primeira rota antes de modelagem final.
