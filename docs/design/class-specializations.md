# Especializações das cinco classes

> **Complemento de direção de 16/10/2026:** os detalhes abaixo foram fechados por delegação do proprietário nas seções “Progressão e parâmetros do piloto” e em [ameaça e formação regional](regional-threat-and-training.md). Elas substituem as pendências iniciais de níveis, respec e atribuição de serviço. Valores novos continuam EXPERIMENTAIS até playtest; não indicam runtime pronto.

**Direção aprovada na conversa de 16/10/2026:** acrescentar escolhas de desenvolvimento dentro de cada classe, com primeira escolha no nível 5, aviso discreto, conversa com NPC no hub e tela de comparação. Bram conduz o Bastião. O nível 5 é a referência inicial para implementação; seu momento na campanha ainda depende da curva de XP e de playtest. Esta ficha não afirma que há árvore, NPC interativo ou efeitos de especialização implementados em Unity.

## Base comum e primeira escolha

A classe mantém suas quatro ativas e duas passivas canônicas. A1 é o ataque gratuito e ocupa um dos quatro nós ativos; especialização não adiciona uma quinta ativa nem altera os controles. A2 no nível 2, P1 no 3, A3 no 4, P2 no 5 e A4 no 6 mantêm suas regras atuais de compra/pré-requisitos.

**Decisão de implementação desta ficha:** a primeira escolha de especialização é uma seleção gratuita separada dos pontos da árvore básica. Ela não consome o ponto necessário para P2 e não impede comprar A4 no nível 6. Não concede pontos de atributo, ouro ou equipamento. Os dois caminhos são mutuamente exclusivos nessa primeira seleção. Investimentos posteriores seguem o orçamento e pré-requisitos definidos na seção de progressão abaixo. IDs e dados executáveis ainda precisam de catálogo antes de ativação.

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

Primeiro piloto: dois caminhos do Bastião usando os mesmos nós básicos. A escolha deve mudar uma decisão de combate perceptível. Exemplo ainda experimental: modificação defensiva do Impacto que prepara contra-ataque versus modificação ofensiva que compromete defesa inimiga. A seção de progressão define condição, alvo, duração, teto, cooldown/proc, tratamento em boss e interação com passivas para o piloto. Os primeiros valores experimentais estão especificados na seção de progressão; ainda não são balanceamento validado.

## Mentor e disponibilidade

Bram é o mentor definido para a primeira escolha do Bastião na Cidadela, aproveitando sua função existente. Doença de secundários, degradação visual e escolhas morais não removem esse serviço. Após nível 5, sua indicação aparece no diário/mapa do hub sem expor consequências futuras da história.

Os responsáveis das outras quatro classes e a continuidade regional estão definidos em [ameaça e formação regional](regional-threat-and-training.md): Irian/Tecelão, Sera/Sentinela, Lyra/Invocadora e Maera/Inquisidor. Eles disponibilizam registros/exercícios adequados à função narrativa existente; isso não inventa biografia de mestre. Hubs posteriores usam responsáveis locais distintos. Uma classe só pode ser anunciada como completa após conversa e serviço estarem funcionais.

## Persistência e redistribuição

Elegibilidade deriva de classe e nível confirmados pelo domínio. Confirmação exige contexto validado de mentor/hub, versão esperada, caminho pertencente à classe e seleção explícita; a UI não se autoriza sozinha. Repetir o mesmo comando retorna seu resultado; um comando com payload diferente conflita. Confirmar, aplicar modificadores e atualizar versão pertencem à mesma transação do personagem.

Seleção e aviso dispensado são estados diferentes, persistidos no agregado do personagem. Usar IDs estáveis revisados em catálogo, nunca rótulo localizado, índice de cartão ou nome de nó. IDs e schema finais ainda não foram definidos; adicionar migração antes de alterar saves existentes. O núcleo de memória de campanha do PR #26 não deve se tornar uma segunda autoridade de progressão de classe.

Respec acontece em hub seguro, com preview e confirmação. Pode trocar a primeira direção e futuros investimentos quando o sistema estiver completo. Preserva classe, nível, atributos-base, escolhas narrativas e habilidades básicas adquiridas. A primeira redistribuição é gratuita e as seguintes seguem o custo experimental e regras da seção abaixo; não criar troca em combate. Remover efeitos antigos e aplicar novos uma única vez, sem cura, reset de cooldown ou ganho econômico por alternância.

## Critérios de aceite e trabalho restante

- Nível 4 bloqueia escolha; níveis 5 e superiores habilitam, inclusive ganho de múltiplos níveis e reload. Escolher depois não bloqueia a campanha.
- Aviso não interrompe combate e sua dispensa persiste. Menu/diário conserva caminho até mentor.
- Confirmação funciona apenas em contexto validado; cancelar não altera personagem. Retry, conflito e falha de save não duplicam seleção/efeito.
- Compra dos cinco nós básicos até nível 6 continua possível: especialização não consome seus pontos. Seleção não concede A3/A4 que não foram compradas.
- Dois caminhos do Bastião demonstram comportamentos distintos e não introduzem teclas adicionais; nenhuma alteração fora dos tetos do combate.
- Save/reload mantém seleção, efeitos e aviso; respec troca efeitos atomicamente e não modifica a memória moral da campanha.
- Responsáveis permanecem acessíveis apesar da degradação do hub; o serviço regional recupera escolhas pendentes sem depender do popup.
- Curva de XP confirma se nível 5 oferece tempo suficiente para experimentar a classe e sentir a escolha ainda no Ato I. Sem medir, não prometer posição exata no mapa ou tempo em minutos.

Próximos passos: converter os parâmetros abaixo em catálogo com IDs revisados; implementar domínio com testes; integrar UI/mentor e agregado persistente no cliente Unity após sua inicialização; playtest antes de expandir o sistema às outras classes. Esta entrega registra o design e seus contratos, sem criar gameplay ou fechar C0.

## Progressão e parâmetros do piloto — direção delegada

Os números desta seção são baseline **EXPERIMENTAL de design**. Não alterar o combate Godot automaticamente nem anunciar balanceamento validado.

| Nível | Especialização |
|---|---|
| 5 | Escolher direção principal e receber seu nó inicial gratuitamente |
| 10 | Ganhar 1 ponto de especialização; pode abrir o nó inicial da outra direção |
| 15 | Ganhar 1 ponto |
| 20 | Ganhar 1 ponto |
| 25 | Ganhar 1 ponto |
| 30 | Ganhar 1 ponto |

Cada direção tem quatro nós de um rank, em cadeia: inicial (nível mínimo 5), desenvolvimento (10), aprofundamento (20) e culminação (30). Cada nó posterior exige o anterior da mesma direção e custa um ponto; o inicial da direção secundária também custa um ponto e só pode ser comprado a partir do nível 10. São oito nós possíveis e seis investimentos totais, incluindo a seleção gratuita: não é possível maximizar os dois caminhos. Exemplos no nível 30: 4+2 ou 3+3. O personagem pode guardar pontos; pontos e desbloqueios atrasados são recuperados do nível, não perdidos por deixar de abrir a tela.

Os pontos são separados de atributos e dos cinco nós básicos compráveis. Compra da direção não compra habilidade básica nem muda suas teclas. A escolha principal determina o nó inicial gratuito; respec pode trocar essa direção e reiniciar a distribuição. Não receber uma nova gratuidade/ponto a cada troca. Textos e catálogo dos nós posteriores devem ser fechados por classe antes da ativação; esses marcos não concedem bônus numéricos implícitos.

### Bastião — primeiro nó defensivo

Rótulo de produção **EXPERIMENTAL: Contra-ataque do Baluarte**, sem ID persistível aprovado nesta ficha.

- Um bloqueio frontal confirmado durante Guarda do Crepúsculo prepara uma única carga por 3 segundos de simulação.
- O próximo Impacto do Baluarte validado consome a carga e recebe +15% de dano direto, aditivo com outros bônus dentro do teto global de +100%. Não modifica poder-base permanentemente, cura ou DoT.
- A carga é consumida uma vez por instância de ação validada, mesmo que não alcance alvo. Retry não consome novamente; comando recusado mantém a carga até expirar. Todos os alvos válidos desse mesmo Impacto usam o mesmo snapshot, respeitando um acerto por alvo.
- Cooldown interno de 6 segundos contado da preparação; novos bloqueios não acumulam nem renovam a carga durante esse cooldown. Encerrar encontro, morrer ou trocar especialização remove a carga; reload preserva cooldown/tempo restante sem restaurar uma carga consumida.
- Postura Firme continua com sua própria regra, sem alterar teto de redução de 60%. Não acrescenta stun contra boss. Requer Guarda e Impacto adquiridos para uso efetivo; a escolha não os concede.

### Bastião — primeiro nó ofensivo

Rótulo de produção **EXPERIMENTAL: Ruptura do Baluarte**, sem ID persistível aprovado nesta ficha.

- Um Impacto do Baluarte que causou dano direto confirmado aplica ruptura por 5 segundos: reduz DEF física efetiva em 15% em comuns/elites e 7,5% em subchefes/bosses.
- O próprio golpe que aplica o efeito usa a defesa anterior; os ataques posteriores podem aproveitar a ruptura. Resistências e MDEF não mudam. DEF final é inteira, não negativa, calculada com floor; efeito não reescreve a base do ator.
- Mesmo tipo não acumula entre fontes: prevalece a maior redução ativa, e reaplicação válida apenas renova sua duração. Outros debuffs de defesa precisarão de política de composição explícita antes de existir; não multiplicar reduções sem limite.
- Erro de alvo, imunidade ou dano de HP/barreira não confirmado não aplica ruptura. Barreira conta como contato com dano confirmado, desde que o resultado registre absorção positiva. Considerar dano positivo a HP OU barreira; animação e contato visual sozinhos não bastam.
- Preserva cooldown, custo MP e limites de acerto do Impacto, sem stun extra em boss. Não reduz DEF duas vezes por retry, DoT ou VFX.

As duas propostas modificam uma habilidade existente; nenhuma exige arma lendária, tecla adicional ou nova animação completa para ser testada. Reutilizar guarda/impacto com sinais visuais e sonoros distinguíveis. Efeitos nos inimigos continuam respeitando duração até o reset do encontro; respec no hub não permite manter bônus pessoais antigos.

### Respec e equipamento

Primeira redistribuição efetivamente confirmada: gratuita por personagem. Depois: **5 × nível atual em ouro**, baseline experimental (25 no nível 5, 150 no 30). Preço mostrado antes de confirmar; a primeira seleção não consome a gratuidade. Cancelamento, inspeção de árvore e tentativa recusada não cobram. Alterar direção ou distribuição numa mesma confirmação constitui uma redistribuição; não cobrar por clique de nó.

Permitido somente em hub seguro, fora de combate, sem ação transitória pendente. Gravar débito, contador de gratuidade, seleção e distribuição na mesma transação. Preservar recursos atuais limitados aos novos máximos, cooldowns e estados de uso; não curar, regenerar, renovar invocação ou reiniciar habilidade. A política final de migração precisa cobrir novos máximos e efeitos antes de integração.

Nenhuma direção exige drop específico. No piloto, defesa valoriza sobrevivência/posicionamento com equipamento comum; ruptura valoriza ataques físicos e janela de pressão. Lendários podem ampliar possibilidades depois, respeitando tetos, sem serem chave obrigatória de viabilidade. Não criar templates ou preços de equipamento por esta ficha.

### Validação específica

Testar fronteiras 4/5/9/10/15/20/25/30, aquisição atrasada, teto de seis investimentos, cadeia de nós, duas culminações impossíveis e exemplos 4+2/3+3. No Bastião: bloqueio traseiro não prepara carga; dois bloqueios não acumulam; ação recusada não consome; retry/área não duplicam; expiração usa simulação; ruptura não altera o primeiro hit nem acumula; bosses recebem regra reduzida sem stun extra. Respec gratuito/cobrado, bolsa sem ouro, falha de save, retorno de cooldown e reload exigem testes.

Critério de playtest: com equipamento comum, ambos os caminhos completam os encontros do piloto; um recompensa bloquear e responder, outro criar/aproveitar janela ofensiva. Medir sobrevivência, DPS e clareza da condição, sem declarar equivalência por percentuais isolados. Não ativar os nós enquanto os testes de domínio e integração não existirem.
