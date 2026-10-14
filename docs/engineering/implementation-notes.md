# Integração da primeira fatia
Autorização do usuário: implementar automaticamente o jogo jogável em 14/10/2026 (sessão local). Trabalho integrado na branch codex/playable-first-slice; cartões executados por dependência, sem merge automático.

## Contratos complementares
- Cada saída do hub inicia uma expedição numerada persistida. source_id = forest_01.<enemy>.<nn>:<expedition>. Mortes comuns são únicas nessa expedição; retorno/reload acontece no hub. Só uma nova saída inicia outro ciclo. Boss e decisões são únicos por campanha.
- Excedente de loot vai para pending_loot persistido na mesma transação da recompensa, retirável no hub sem novo sorteio. Nunca descartar silenciosamente.
- ResourceRegen recebe intelligence e bloqueio de inimigo engajado/DoT. Os relógios de 3 e 5 segundos são independentes e a fração elegível do passo é contabilizada.
- Investigar pegadas, relatos e poço desbloqueia a escolha na aldeia; nenhum fato é diagnóstico conclusivo. IDs: forest_01.clue.tracks, forest_01.clue.testimony, forest_01.clue.well.
- Escolha confirmada agenda first_return_to_citadel. O retorno aplica hub.memorial_observed ou hub.contamination_observed e hub.shelter_observed. Repetição nunca reaplica.
- hub_stage continua 0 no mapa 1: só mapas 3/6/9/11 concluídos permitem estágios 1/2/3/4 (GDD 08). Mudanças locais usam flags observadas, não incremento de estágio.
- Critério do slice: derrotar Garras, investigar, decidir e retornar ao hub; não equivale a completar os quatro atos.
- Arte e som provisórios originais gerados pelo próprio projeto; sem plugins ou assets externos.
- Referência multiplayer anexada é histórica: rede, Network e GlobalSignals não substituem o contrato offline aprovado.
