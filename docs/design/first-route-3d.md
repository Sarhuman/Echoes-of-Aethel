# Primeira rota 3D — Cidadela e Bosque das Névoas

**Estado:** briefing de produção para Unreal, não cena pronta. O [manifesto JSON](../engineering/first-route-content.json) é o índice de IDs desta rota. História e consequências seguem [GDD 04](../GDD/04_mapas_e_quests.md) e [GDD 07](../GDD/07_roteiro_e_dialogos.md); valores de combate/itens seguem [valores experimentais](../engineering/experimental-values.md). Uma planta em metros, posições e custos de produção só serão fixados após blockout no editor.

## Percurso e leitura espacial

```text
Cidadela (checkpoint seguro)
  praça/cristal — Irian e conselho
  oficina de Bram (interior seguro, serviço)
  Lyra (cura/consumíveis) · Sera (loja) · Maera (portão)
  portão leste ⇄ trilha do Bosque
Bosque (sem checkpoint)
  trilha e primeiro Lobo → três pistas externas → aldeia
  casa abandonada → porão (exploração opcional)
  Cultista → decisão da aldeia → arena de Garras de Musgo
  retorno pelo portão → Cidadela alterada pela escolha
```

Setas indicam relações, não um corredor único nem ordem obrigatória entre decisão e luta. As três pistas existentes (`forest_01.clue.tracks`, `.testimony`, `.well`) continuam **fora** da casa e disponíveis antes de decidir. O acesso à casa acrescenta exploração e contexto, sem criar quarta pista obrigatória. A arena mantém espaço para evitar varredura, investida e raízes; obstáculos e abertura de fuga devem ser legíveis pela câmera elevada. A borda do abismo tem volume de colisão e silhueta visível. Nenhum inimigo pode bloquear permanentemente o jogador contra poço, porta ou cerca.

## Locais, serviços e interiores

| Local | Função | Entrada/saída | Segurança e estado |
|---|---|---|---|
| `citadel` | Hub, cristal, conselho, portão, comércio e reserva de loot | Portão para `forest_01`; interiores retornam ao mesmo setor | Checkpoint único; sem dano nem inimigos ativos |
| `citadel.bram_workshop` **NOVO/EXPERIMENTAL** | Interior de oficina e contexto de Bram; futura forja | Porta visível junto à oficina; saída retorna à porta externa | Seguro; o serviço e a conversa com Bram continuam acessíveis na praça até o interior estar funcional |
| `forest_01` | Trilha, aldeia, pistas, encontros e arena | Portão para Cidadela; porta da casa e arena | Combate permitido; morte retorna à Cidadela sem apagar decisões/coletas |
| `forest_01.abandoned_house` **NOVO/EXPERIMENTAL** | Primeira exploração interna, nota ambiental e suprimento | Porta acessível na aldeia; saída na mesma porta | Um recipiente persistente; sem inimigo obrigatório |
| `forest_01.root_cellar` **NOVO/EXPERIMENTAL** | Porão curto sob a casa, raízes ligando a aldeia ao bosque | Escada visível na casa; volta à mesma escada | Um recipiente persistente; sem checkpoint ou boss adicional |

Toda entrada mostra um sinal de interação, destino e retorno. A câmera e os volumes mantêm jogador, inimigos e saída visíveis; atravessar porta não concede loot por si só. Se transição/carregamento falhar, o jogador fica no local anterior sem alterar inventário, quest ou checkpoint. No hub, Bram, Lyra, Sera, Maera e Irian permanecem alcançáveis independentemente da escolha da quest. A oficina não substitui o acesso a Bram no blockout enquanto o interior não estiver pronto.

## Recompensas colocadas à mão

| Fonte | Conteúdo fixo **NOVO/EXPERIMENTAL** | Regra |
|---|---|---|
| `reward.forest_01.abandoned_house.cache` | 1 `potion_health_small` | Primeiro saque da campanha; coletar uma vez |
| `reward.forest_01.root_cellar.cache` | 1 `antidote_flask` | Primeiro saque da campanha; coletar uma vez |

Esses dois suprimentos usam itens já catalogados e **não** substituem drops dos inimigos, recompensa de Garras ou opções da quest. O recipiente só muda de fechado para aberto após gravação atômica de item/reserva e `granted_rewards[reward_id]`. Se a bolsa estiver cheia, o item vai para `pending_loot` e a coleta é registrada no mesmo commit. Se o commit falhar, nenhum estado visual ou econômico é confirmado. Reentrar, morrer e recarregar não recriam o item. Preço, peso e efeito vêm do catálogo existente; não duplicar números nesta ficha.

## Elenco e encontros

| Papel | Identidade/ID existente | O que a prova precisa mostrar |
|---|---|---|
| Herói | `twilight_bastion` | Movimento 3D sem deslizamento, machado, Guarda, Impacto, Fenda e esquiva |
| Conselho/serviços | `npc.elder` Irian, `npc.blacksmith` Bram, `npc.alchemist` Lyra, `npc.merchant` Sera, `npc.gatekeeper` Maera | Presença visual, interação e função coerentes; variantes por escolha |
| Comuns | `ethereal_wolf`, `blind_cultist` | Silhuetas, locomoção enraizada no chão, antecipação e separação física |
| Subchefe | `moss_claws` | Arena, três padrões, recompensa única e saída desbloqueada |
| Quest | `act1.spy_hunt` | Três pistas, `judge_suspects` ou `shelter_villagers`, consequência visível no retorno |

Silvano é o **boss do mapa 03**, não da primeira rota; Matriarca das Águas é subchefe do mapa 02. As outras quatro classes e seus diálogos existem no GDD, mas não entram na primeira prova jogável.

## Pacote mínimo de produção 3D

**Cenário:** kit modular de piso, parede, escada, arco, portão e ruína da Cidadela; cristal, oficina, bancas e fontes; trilha, árvores, pedras rúnicas, cerca, poço, casa/porão e arena do Bosque. Blockout primeiro; materiais, luz, névoa e VFX depois de colisão/navegação aprovadas. Nenhuma peça 3D está pronta no repositório.

**Atores:** modelo/rig do Bastião com machado e escudo; cinco NPCs distinguíveis; Lobo, Cultista e Garras com pivô, cápsula e animações de idle, deslocamento, ataque, reação e morte. Os PNGs atuais servem como referência de design, não como mesh, rig ou animação 3D.

**Cinemática e som:** prólogo curto com enquadramentos da Cidadela, Maera e o cristal, seguido da primeira conversa com Irian. Texto em português, música original e avanço/pulo equivalentes. Voz sintética automática fica fora; eventual dublagem exige gravação dirigida e proveniência. Sons de passos, portas, golpes, ambiente e transições precisarão de mixagem própria. Não usar material de Diablo ou de outras franquias.

Para cada asset final, registrar origem, criador, licença/cessão, arquivo-fonte, versão e transformação. Imagens conceituais não são capturas nem conteúdo pronto para Unreal.

## Aceite da primeira prova

- É possível sair da Cidadela, entrar e sair da oficina, ir ao Bosque, entrar e sair da casa e do porão e voltar pelo portão sem atravessar paredes ou perder a câmera.
- O jogador encontra as três pistas externas, escolhe uma rota, derrota Garras e retorna; decisão, boss e dois recipientes persistem após save/reload, morte e repetição de entrada.
- Bolsa cheia preserva os suprimentos na reserva; falha de gravação não abre recipiente nem consome recompensa. Compra/venda na Cidadela não duplica ouro ou item.
- O prólogo assistido e o pulado produzem o mesmo estado inicial. A Cidadela segue segura; serviços essenciais continuam acessíveis após ambas as escolhas.
- A build Windows abre fora do editor; desempenho e legibilidade são medidos no PC de teste antes de ampliar o mapa. Resultado e limitações ficam registrados no PR.
