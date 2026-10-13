# Design canônico

## Identidade e escopo

Título: **Echoes of Aethel**. Fantasia sombria com magia antiga decadente. Combate em tempo real, exploração, equipamentos e especialização de habilidades. A influência de ARPGs orienta o ritmo; personagens, mapas e arte serão próprios.

Direção proposta para o protótipo: top-down 2D, pixel art e iluminação dinâmica limitada. A perspectiva isométrica fica como alternativa a avaliar antes da produção de assets finais. Primeira entrega single-player offline, sem contas, backend ou multiplayer.

Loop: preparar equipamento no hub → explorar → lutar e coletar → decidir um dilema → enfrentar um sub-boss → voltar e observar consequências → avançar. A campanha final contém quatro atos de três mapas; não exige mundo aberto contínuo no MVP.

## Cinco classes

| ID estável | Classe | Combate e recurso temático | Origem e conflito |
|---|---|---|---|
| rune_weaver | Tecelão de Runas | Magia elemental de área, barreiras, controle; Inteligência/Éter | Sua ordem rompeu nós da realidade e foi consumida. Busca selar as fendas e redimir seus mestres. |
| shadow_sentinel | Sentinela das Sombras | Adagas, armadilhas, esquiva, ocultação breve; Destreza/Precisão | Uma execução ativou um selo. A velocidade recebida consome sua vitalidade; busca uma cura. |
| twilight_bastion | Bastião do Crepúsculo | Machado, escudo, defesa e controle; Força/Vitalidade | Fechou os portões para salvar civis, abandonando família e soldados. Sua proteção nasce da culpa. |
| thorn_summoner | Invocadora de Espinhos | Feras espirituais, golens, veneno e suporte; Sabedoria/Vontade | A tribo fundiu suas almas à natureza; ela carrega as vozes ancestrais no cajado. |
| sacred_flame_inquisitor | Inquisidor da Chama Sagrada | Maça, fogo, autocura; Fé/Força | Renegou a seita ao descobrir a aliança de seu líder com a corrupção. |

No protótipo, implementar somente o Bastião: ataque básico, defesa com custo de recurso e esquiva. Provocação influencia inimigos controlados por IA; não pressupõe multiplayer. As outras classes são conteúdo planejado. Árvores, coeficientes e custos ainda não estão balanceados.

## Combate, loot e progressão

- Ataques fortes têm antecipação visual, área legível e recuperação. Colisões determinam acertos independentemente da animação.
- Inimigos comuns: XP pequena e chance de ouro, consumíveis e equipamento comum/raro.
- Elites e sub-bosses: XP maior, materiais e maior chance de raros/épicos.
- Chefes de ato: ao menos um equipamento épico; lendário tem chance adicional, sem promessa de lendário em toda repetição. Fragmento de campanha somente na primeira vitória.
- Raridades: comum, raro, épico, lendário. Conjuntos e crafting avançado ficam para expansão.
- Respawn ocorre em nova expedição ou reset explícito de zona. Não reaparecer sobre o jogador nem restaurar decisões de história.
- Morte retorna ao último hub/checkpoint seguro. No MVP, preserva escolhas e inventário e reinicia o encontro sem nova recompensa de campanha.

XP, tabelas de drop, níveis por mapa e limites de modificadores precisam de testes de balanceamento; os termos baixa/alta são intenção relativa, não valores implementados.

## Hubs seguros

A Cidadela dos Últimos é o hub principal. Acampamentos seguros dos atos seguintes oferecem viagem e serviços equivalentes. Ferreiro, alquimista, encantador e baú são a visão final; no MVP bastam cura e inventário.

Não há combate, dano ambiental ou invasão jogável em hubs. Doença, fanatismo e escassez mudam arte, diálogo e personagens. Serviços essenciais permanecem acessíveis por substitutos. Ataques narrativos atingem assentamentos externos, nunca violam a segurança mecânica do hub.

## Decisões sombrias

Cada dilema narrativo oferece exatamente duas opções e ambas têm um custo trágico. A regra não se aplica a menus, tutorial ou interações operacionais. As alternativas precisam mudar quem é salvo, quem sofre, encontros e epílogo; alimentar o mesmo ritual não torna suas consequências idênticas.

O resultado exato pode ficar oculto, mas pistas antecipam o risco: sintomas, inscrições, relatos ou alertas. O diário registra o que foi observado, sem revelar eventos futuros. Evitar punições arbitrárias que simplesmente reduzam loot/XP ou dobrem a vida de bosses.

Afinidades invisíveis separadas: misericórdia e julgamento. Ambas aumentam a carga ritual; não são uma barra de bom/mau. Misericórdia pode favorecer regeneração/proteção, julgamento pode favorecer ofensiva com custo de recurso. Bônus, custos e acumulação terão limites; nenhuma combinação pode inviabilizar a campanha.

Consequências são aplicadas uma vez, podem ser imediatas ou disparadas na entrada de um mapa/retorno ao hub, e permanecem após carregar o save.
