# Especializações das cinco classes

**Direção aprovada na conversa de 16/10/2026:** acrescentar escolhas de desenvolvimento dentro de cada classe, com primeira escolha no nível 5, aviso discreto, conversa com NPC no hub e tela de comparação. Bram conduz o Bastião. O nível 5 é a referência inicial para implementação; seu momento na campanha ainda depende da curva de XP e de playtest. Esta ficha não afirma que há árvore, NPC interativo ou efeitos de especialização implementados em Unity.

## Base comum e primeira escolha

A classe mantém suas quatro ativas e duas passivas canônicas. A1 é o ataque gratuito e ocupa um dos quatro nós ativos; especialização não adiciona uma quinta ativa nem altera os controles. A2 no nível 2, P1 no 3, A3 no 4, P2 no 5 e A4 no 6 mantêm suas regras atuais de compra/pré-requisitos.

**Decisão de implementação desta ficha:** a primeira escolha de especialização é uma seleção gratuita separada dos pontos da árvore básica. Ela não consome o ponto necessário para P2 e não impede comprar A4 no nível 6. Não concede pontos de atributo, ouro ou equipamento. Os dois caminhos são mutuamente exclusivos nessa primeira seleção. Poder misturar investimentos posteriores é uma direção de design; orçamento, níveis, pré-requisitos e limites ainda precisam de catálogo antes de ativação.

Não há requisito de afinidade moral, escolha de quest, drop aleatório ou conclusão do ato para essa primeira seleção. Escolher depois não impede ganhar XP ou avançar na campanha. Pular do nível 4 para o 6 ou carregar um personagem elegível também disponibiliza a escolha; não depender apenas de capturar o evento exato de level-up.

## Fluxo no jogo

1. Desde o nível 1, o menu do personagem permite consultar as duas direções da classe. Antes do 5, apresenta “Especialização disponível no nível 5”. Nada precisa ser comprado para visualizar.
2. Ao chegar ao nível 5 sem especialização escolhida, um aviso discreto informa a disponibilidade. Para o Bastião: “Uma nova especialização está disponível. Procure Bram na Cidadela.” O aviso não pausa o combate, captura input ou abre diálogo. Ele pode ficar pendente enquanto houver boss, cutscene ou outra mensagem prioritária.
3. O diário/menu conserva a orientação depois que o aviso some. Reload não repete avisos já dispensados. Reconhecer o aviso não seleciona especialização; falha de gravação permite uma nova tentativa sem perder a oportunidade.
4. O jogador conversa com o mentor em um hub seguro e escolhe “Conhecer especializações”. Uma conversa curta contextualiza os estilos; o roteiro final ainda precisa de revisão, sem inventar falas canônicas nesta ficha.
5. A tela mostra os dois caminhos lado a lado, habilidades existentes afetadas, descrição concreta do comportamento, condições, limites e exemplo de uso. A comparação deve permitir distinguir proteção/contra-ataque de pressão/ruptura no Bastião. Não substituir explicação por apenas “+dano” ou uma nota abstrata de poder.
6. Selecionar um cartão abre a confirmação; voltar/cancelar não gasta recursos nem altera estado. Texto de interface proposto: “Confirmar especialização? Você poderá redistribuir sua especialização em um hub seguro.” Disponibilizar essa promessa somente quando o respec estiver funcional.
7. Após confirmação persistida, apresentar uma explicação breve da modificação inicial e realçar a habilidade afetada no menu. Se ela ainda não foi comprada, informar o pré-requisito; a seleção nunca concede ou compra habilidades implicitamente. Para o Bastião, priorizar uma modificação de Guarda/Impacto já acessível nessa etapa.

Com controle ou teclado, navegação tem foco visível, ordem previsível e cancelar explícito. Texto ampliável; cores não são o único indicador de seleção. A tela pode pausar simulação/timers no offline conforme regra existente de diálogo. Tooltip/menu consultado em combate não deve criar pausa parcial que deixe cooldowns avançarem sozinhos.

## Caminhos de desenvolvimento

As identidades abaixo são **propostas de conteúdo**, não nomes definitivos de subclasses, IDs persistíveis ou efeitos liberados. Não alterar silenciosamente as habilidades e números do GDD 01.

| Classe / ID existente | Direção 1 proposta | Direção 2 proposta | Limite de produção |
|---|---|---|---|
| Bastião / `twilight_bastion` | Defesa frontal, bloqueio e contra-ataques | Golpes comprometidos, ruptura e pressão | Guarda e Impacto são candidatos à primeira modificação; respeitar teto de redução e imunidade de boss a stun |
| Tecelão / `rune_weaver` | Controle de terreno e efeitos persistentes | Dano direto e combinações de magia | Não criar stacks ilimitados, crítico de DoT ou regeneração em combate |
| Sentinela / `shadow_sentinel` | Precisão com adagas e eliminação de alvo prioritário | Mobilidade, armadilhas e emboscadas | Preservar kit de combate curto; caminho de arco/distância exigiria decisão de armas/animações separada |
| Invocadora / `thorn_summoner` | Comando e fortalecimento das criaturas | Uso deliberado de invocações como recurso para efeitos | Consumo de invocação é NOVO/EXPERIMENTAL; reinvocar no baseline continua sem explosão/recompensa e mantém limite de entidades |
| Inquisidor / `sacred_flame_inquisitor` | Proteção, autocura e sustentação | Marcas, julgamento e execução | Marcas/execução são propostas; não conceder imunidade, cura ilimitada ou converter afinidade moral em requisito |

Primeiro piloto: dois caminhos do Bastião usando os mesmos nós básicos. A escolha deve mudar uma decisão de combate perceptível. Exemplo ainda experimental: modificação defensiva do Impacto que prepara contra-ataque versus modificação ofensiva que compromete defesa inimiga. Antes de implementar, definir condição, alvo, duração, teto, cooldown/proc, tratamento em boss e interação com passivas. Não há valores aprovados para esses efeitos nesta ficha.

## Mentor e disponibilidade

Bram é o mentor definido para a primeira escolha do Bastião na Cidadela, aproveitando sua função existente. Doença de secundários, degradação visual e escolhas morais não removem esse serviço. Após nível 5, sua indicação aparece no diário/mapa do hub sem expor consequências futuras da história.

Os mentores das outras quatro classes ainda precisam ser atribuídos entre NPCs existentes conforme função narrativa. Não criar cinco personagens novos ou atribuir a Bram domínio de todas as classes por conveniência. A elegibilidade por nível independe desse conteúdo; uma classe só pode ser anunciada como completa após seu mentor, diálogo e serviço estarem funcionais.

## Persistência e redistribuição

Elegibilidade deriva de classe e nível confirmados pelo domínio. Confirmação exige contexto validado de mentor/hub, versão esperada, caminho pertencente à classe e seleção explícita; a UI não se autoriza sozinha. Repetir o mesmo comando retorna seu resultado; um comando com payload diferente conflita. Confirmar, aplicar modificadores e atualizar versão pertencem à mesma transação do personagem.

Seleção e aviso dispensado são estados diferentes, persistidos no agregado do personagem. Usar IDs estáveis revisados em catálogo, nunca rótulo localizado, índice de cartão ou nome de nó. IDs e schema finais ainda não foram definidos; adicionar migração antes de alterar saves existentes. O núcleo de memória de campanha do PR #26 não deve se tornar uma segunda autoridade de progressão de classe.

Respec acontece em hub seguro, com preview e confirmação. Pode trocar a primeira direção e futuros investimentos quando o sistema estiver completo. Preserva classe, nível, atributos-base, escolhas narrativas e habilidades básicas adquiridas. Custo, primeira redistribuição e regras de cooldown precisam de decisão econômica; não cobrar valor arbitrário ou criar troca grátis em combate. Remover efeitos antigos e aplicar novos uma única vez, sem cura, reset de cooldown ou ganho econômico por alternância.

## Critérios de aceite e trabalho restante

- Nível 4 bloqueia escolha; níveis 5 e superiores habilitam, inclusive ganho de múltiplos níveis e reload. Escolher depois não bloqueia a campanha.
- Aviso não interrompe combate e sua dispensa persiste. Menu/diário conserva caminho até mentor.
- Confirmação funciona apenas em contexto validado; cancelar não altera personagem. Retry, conflito e falha de save não duplicam seleção/efeito.
- Compra dos cinco nós básicos até nível 6 continua possível: especialização não consome seus pontos. Seleção não concede A3/A4 que não foram compradas.
- Dois caminhos do Bastião demonstram comportamentos distintos e não introduzem teclas adicionais; nenhuma alteração fora dos tetos do combate.
- Save/reload mantém seleção, efeitos e aviso; respec troca efeitos atomicamente e não modifica a memória moral da campanha.
- Mentores permanecem acessíveis apesar da degradação do hub. As quatro atribuições restantes são pendências explícitas.
- Curva de XP confirma se nível 5 oferece tempo suficiente para experimentar a classe e sentir a escolha ainda no Ato I. Sem medir, não prometer posição exata no mapa ou tempo em minutos.

Próximos passos: definir catálogo das duas modificações iniciais do Bastião, IDs, efeitos e respec; implementar domínio com testes; integrar UI/mentor e agregado persistente no cliente Unity após sua inicialização; playtest antes de expandir o sistema às outras classes. Esta entrega registra o design e seus contratos, sem criar gameplay ou fechar C0.
