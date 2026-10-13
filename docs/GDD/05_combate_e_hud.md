# Sprint 5 — Combate, recursos, dificuldade e HUD

**Projeto:** Echoes of Aethel · **Versão:** 1.0 · **Estado:** especificação para protótipo; combate, HUD e Eclipse ainda não implementados.

Este módulo define a engenharia do combate em tempo real com apresentação 16-bit. Complementa as [classes](01_classes_e_atributos.md), a [API autoritativa](02_arquitetura_tecnica.md), o [loot](03_sistema_de_loot.md) e os [mapas/quests](04_mapas_e_quests.md). Prevalece sobre regras anteriores de recurso do Bastião, regeneração e multiplicador crítico. O roteiro final detalhado de Vael continua pendente; esta Sprint passou a tratar combate e HUD.

## 1. Dificuldade dinâmica e ambiental

Não existe seletor Fácil/Médio/Difícil. O desafio vem de progressão por zona, composição de encontros e consequências das decisões. Recursos de acessibilidade visual/input não alteram recompensa nem são rótulos de dificuldade.

### 1.1 Progressão natural por atos

Para um mesmo arquétipo de inimigo, baseline experimental de crescimento linear: `HP_base = HP_ato1 + 15×(ato−1)` e `P_base = P_ato1 + 3×(ato−1)`. `HP_ato1` e `P_ato1` pertencem ao catálogo. DEF/MDEF e padrões são declarados por arquétipo, sem multiplicador oculto extra. Não aplicar esse crescimento novamente sobre um template já ajustado ao ato.

| Ato | Incremento HP do arquétipo | Incremento poder de ataque | Desafio ambiental |
|---|---:|---:|---|
| I | +0 | +0 | Investidas, lentidão, áreas previsíveis |
| II | +15 | +3 | Visibilidade limitada e posicionamento |
| III | +30 | +6 | Invocações e zonas de risco com saída |
| IV | +45 | +9 | Sequências combinadas e controle de espaço |

Esses valores servem para testar a função, não provam equilíbrio entre criaturas diferentes. Bosses têm ficha própria e fases; não recebem o crescimento duas vezes.

### 1.2 Corrupção e retorno a mapas anteriores

Todas as escolhas aumentam a carga ritual canônica, inclusive misericórdia. `C` é o número de decisões principais concluídas, entre 0 e 12; não muda por respawn, tempo offline ou repetição de requests. Flags definem aparência/tipos de invasor; o estágio agregado determina o teto do desafio. Assim, consequências de ambas as rotas são reais e não existe ramo moral que escape à Bruma.

| Estágio de Bruma | C | Chance de promover comum a elite em zona revisitada |
|---|---|---:|
| 0 | 0–2 | 0% |
| 1 | 3–5 | 10% |
| 2 | 6–8 | 20% |
| 3 | 9–11 | 30% |
| 4 | 12 | 40%, somente em replay pós-campanha autorizado |

Aplicar somente ao iniciar **nova expedição em mapa já concluído**, nunca no primeiro acesso, no hub ou no meio de um encontro existente. Snapshot de corrupção fica ligado ao encontro. Promoção tem máximo de um monstro por grupo de três e não afeta boss/sub-boss ou elite existente. RNG servidor escolhe e registra os promovidos; desconectar não rerrola o mesmo encontro.

Elite promovido: `HP=floor(HP_base×1,5)`, `P=floor(P_base×1,2)`, intervalo mínimo entre ataques reduzido no máximo 10%. Antecipação visual e recuperação mínima de esquiva não são reduzidas. Sem aumento secreto de alcance/velocidade capaz de tornar o ataque inevitável.

Usar categoria **elite também para loot e XP** conforme Sprint 3; desafio maior não mantém recompensa comum. Alterar combate de nova expedição não bloqueia a rota principal nem desfaz quest. Conduta tirânica pode produzir guardas/espectros mais agressivos; misericórdia pode produzir mutantes/parasitas com os mesmos limites. A Cidadela só muda narrativamente.

### 1.3 Eclipse — desafio pós-campanha

Primeira conclusão validada de qualquer final desbloqueia Eclipse na conta online ou no perfil local offline, sem transferir um desbloqueio local como autoridade online. É uma **nova campanha/personagem com permadeath**, não troca de modo sobre o herói existente. Não contradiz a ausência de seletor tradicional: é um modo de regras distinto, com confirmação explícita da morte permanente.

Morte em Eclipse marca personagem `dead`, encerra encontro e mantém registro/epílogo; não apaga fisicamente auditoria nem permite reviver por reload. Equipamento/XP ganhos antes da morte ficam no registro inacessível como personagem jogável. Morte normal retorna ao checkpoint com escolhas preservadas.

Chefes de Eclipse terão uma fase adicional catalogada, com telegraphs e rota segura. Seus padrões, recompensas próprias e critérios de balanceamento ficam pendentes antes de habilitar o modo. Não gerar fase extra apenas acelerando todos os ataques.

Desconexão não equivale a morte automática nem pausa localmente o servidor: o encontro persiste. Política proposta permite suspensão autoritativa no próximo limite de passo ao detectar perda de sessão, sem conceder invulnerabilidade enquanto conectado, e retomada do mesmo estado/seed; abuso requer monitoramento e teste. Timeout, suspensão e finalização precisam de especificação implementada antes de ativar permadeath online. Se o servidor confirmar morte antes da suspensão, prevalece a morte. Sem promessa de proteção contra todo erro de rede.

## 2. HP, MP, vigor e consumíveis

### 2.1 Recursos

| Recurso | Visual | Regra |
|---|---|---|
| HP | Vermelho #D85B59, número atual/máximo | Ao zerar, morte validada no domínio; normal retorna ao hub, Eclipse encerra o personagem |
| MP / Éter | Azul #69A9F3, número atual/máximo | Custos fixos por conjuração, conforme catálogo |
| Vigor / Fúria | Âmbar #E8B45E, barra auxiliar do Bastião | Reserva 0–100, gerada por combate físico; não é sexto atributo |

HP/MP máximos continuam conforme Sprint 1. MP regenera `1 + floor(INT/10)` por segundo **somente fora de combate**, após 3 s sem gasto de MP e sem interação hostil. Estado fora de combate exige 5 s sem atacar/receber ataque, sem alvo hostil engajado e sem DoT ativo. Usar acumulador inteiro de tempo para não tornar regeneração dependente de FPS. Passivas de MP somam à taxa apenas quando sua condição e a condição fora de combate valem; nunca ignoram o bloqueio por combate.

Não há regeneração natural de HP durante combate. Cura do hub restaura HP/MP e redefine vigor para 30. Vigor ao abrir nova expedição: 30, sem bônus acumulado por reentrar no hub; máximo 100. Fora de combate, após 5 s, converge para 30 a 5 pontos/s (sobe se abaixo e desce se acima). Morte/retry começa em 30, não preserva reserva artificial de 100.

| Geração de vigor do Bastião | Ganho | Proteção contra loop |
|---|---:|---|
| A1 causa dano físico válido em hostil | +8 | Uma vez por comando, não por alvo de AoE |
| Recebe dano físico positivo em HP de hostil | +4 | Cooldown de geração 500 ms, máximo por evento |
| Bloqueia dano físico positivo com A2 | +3 | Uma vez por ataque hostil; cooldown compartilhado de geração defensiva 500 ms |

Não gerar por dano próprio, custo de quest, aliado, cenário, DoT, invocação ou proc. Se o mesmo evento satisfizer receber e bloquear, aplicar somente +4 (ou +3 se não atingiu HP), nunca ambos. Ganho é pós-resolução, limitado ao máximo; não antecipa recurso para pagar a própria habilidade. Ataque básico gratuito e esquiva sempre permitem recuperação funcional.

### 2.2 Custos por classe

| Classe | Recurso das ativas | Custos definidos |
|---|---|---|
| Tecelão | MP | A1 0; Geada 12; Brasas 20; Nó de Éter 18 |
| Sentinela | MP para técnicas de sombra | A1 0; Passo 10; Armadilha 14; Ruptura 18 |
| Bastião | Vigor em técnicas físicas | A1 0; Guarda 8 VG; Impacto 12 VG; Fenda 20 VG |
| Invocadora | MP | A1 0; Golem 22; Jardim 18; Fera 24 |
| Inquisidor | MP em magias sagradas | A1 0; Martelo 12; Vigília 20; Veredito 24 |

A identidade física não migra todas as técnicas da Sentinela para vigor: elas são técnicas mágicas de sombra neste catálogo. Bastião mantém MP para consumíveis e futuras técnicas mágicas, sem novas habilidades sagradas criadas nesta Sprint. “Bola de Fogo 25 MP” é exemplo conceitual, não substitui Círculo de Brasas 20 MP.

Validar custo/cooldown/alvo/estado antes de debitar MP ou vigor. Cooldown só inicia ao executar. Cancelamento antes da execução não cobra; após execução não reembolsa. Retry do mesmo comando retorna resultado existente. Ao debitar custo, manter recurso ≥0.

### 2.3 Alquimista e atalhos

Lysa vende Poção de Vida pequena e Frasco de Éter pequeno com efeitos da Sprint 3 (50 HP / 30 MP). Preços iniciais experimentais: **10 ouro** / **12 ouro**; estoque ilimitado de protótipo, condicionado à capacidade do inventário. Economia será simulada antes de produção.

Compra autoritativa: validar presença no hub, ouro inteiro suficiente, quantidade/capacidade; débito e entrega na mesma transação com idempotência. Nenhuma animação concede item. Não vender em combate nem consumir ouro em compra recusada.

| Ação | Teclado sugerido | Gamepad sugerido | Toque |
|---|---|---|---|
| Poção de Vida | 1 | D-pad esquerda | Botão HP com quantidade |
| Frasco de Éter | 2 | D-pad direita | Botão MP com quantidade |

Atalhos remapeáveis. Gamepad usa borda de pressionamento, não consome a cada frame enquanto segurado; teclado ignora auto-repeat. Vida/MP compartilham cooldown de 5 s. Recurso cheio não consome nem inicia cooldown. Recurso vazio no inventário mostra feedback discreto. Uso validado aplica no próximo passo de simulação; não espera a animação terminar. Sem estoque de vigor instantâneo criado por poção de MP.

Quest do mapa 11 continua afetando MP máximo, não vigor. Essa consequência pode variar em peso mecânico por classe; balancear antes de publicar campanha, sem converter silenciosamente um recurso no outro.

## 3. Backend Math Model

### 3.1 Parâmetros e arredondamento

Fórmulas usam inteiros, pontos percentuais/basis points e `floor` em etapas explícitas. `P` é poder da habilidade, `W` poder de arma/foco compatível, `A` atributo de escala, `L` nível. Não somar arma e foco no mesmo dano. Uma execução tem ID e acertos identificados por alvo; ticks/procs não criticam.

### 3.2 Dano físico

```latex
A_{base} = P + 2A + 2(L-1)
B = A_{base} + W
D_{armadura} = B \times \left(1-\frac{DEF}{DEF+100}\right)
              = B \times \frac{100}{DEF+100}
```

`A=STR` para machado/maça, `A=DEX` para adaga. A redução tem retorno decrescente: DEF 0/100/300 reduz 0%/50%/75% do dano antes de resistências e demais limites. Não produz imunidade absoluta por armadura finita.

### 3.3 Dano mágico e resistência

Para preservar foco, nível e a defesa mágica da Sprint 1, “poder mágico da habilidade” inclui a parte catalogada e seus bônus antes do atributo final:

```latex
P_{magico} = P + W + A + 2(L-1)
B_{magico} = P_{magico} + A
R_{efetiva} = 1 - \frac{100}{100+MDEF}\,(1-R_{elemental})
D_{magico} = (P_{magico}+A)\,(1-R_{efetiva})
```

Magia arcana, gelo, fogo, veneno e invocação usam INT; sagrado usa FTH, evitando tornar a classe de Fé dependente de INT para dano. `R_elemental` é fração −0,50…0,75 após equipamento. `R_efetiva` combina MDEF e resistência, não é uma segunda resistência a aplicar depois. Na implementação inteira, executar MDEF primeiro e resistência depois como pipeline abaixo; não calcular a fração e arredondar em outro ponto.

DEF/MDEF continuam derivados conforme Sprint 1. Imunidade específica é estado explícito do encontro e produz 0; não nasce de resistência acima do teto por erro de empilhamento.

### 3.4 Crítico e pipeline final

```latex
p_{crit} = \min(25\%,\ 5\% + 0{,}25\%\times DEX + bonus_{crit})
m_{crit} = \min(2{,}0,\ 1{,}5 + bonus_{multiplicador})
```

Chance em basis points: `min(2500, 500 + 25×DEX + bonus_crit_bp)`. `bonus_multiplicador` é expresso em pontos, não bônus percentual multiplicado de novo. Raro/Épico/Lendário podem oferecer afixo catalogado de multiplicador; total máximo +0,5, chegando a 2×. Raridade sozinha não garante 2×. Adicionar esse afixo ao catálogo permitido da Sprint 3 antes de gerar itens.

| Etapa | Operação canônica |
|---:|---|
| 1 | Validar acerto por alcance, colisão, direção e esquiva; não há rolagem aleatória de MISS |
| 2 | Construir base física ou mágica; aplicar bônus de dano aditivo 0…100%, com floor |
| 3 | Sortear crítico uma vez para dano direto e multiplicar por 1,5…2,0, com floor |
| 4 | Aplicar `floor(dano×100/(100+defesa))`, DEF físico / MDEF mágico |
| 5 | Aplicar resistência do tipo: `floor(dano×(10000−resistencia_bp)/10000)` |
| 6 | Aplicar redução temporária limitada a 60%, com floor |
| 7 | Acerto válido sem imunidade: mínimo 1. Esquiva/imunidade: 0 |
| 8 | Absorver em barreira, depois subtrair do HP; registrar dano efetivamente absorvido e aplicado |

Apesar da descrição visual de “crítico no dano final”, a matemática faz o multiplicador antes da mitigação, como a Sprint 1, para preservar arredondamento determinístico. Não multiplicar de novo o número mostrado. Cura usa fórmula de FTH já definida, sem crítico; ticks usam o mesmo pipeline sem crítico e respeitam limites de fontes/stack.

### 3.5 Vetores de exemplo

| Cenário | Entrada relevante | Resultado |
|---|---|---:|
| Físico normal | P=10, W=8, STR=12, L=1; DEF=20 | base 42 → 35 |
| Físico crítico 1,5× | Mesma base e defesa | 42 → 63 → 52 |
| Físico crítico 2× | Mesma base e defesa | 42 → 84 → 70 |
| Físico e guarda 40% | Normal 35 | 21 |
| Gelo normal | P=18, W=6, INT=14, L=1; MDEF=20; resistência 25% | base 52 → 43 → 32 |
| Sagrado sem defesa | P=16, W=6, FTH=14, L=1; MDEF=0; resistência 0 | 50 |

Aplicar limites de HP: com alvo de 10 HP, golpe nominal 35 registra `hp_damage=10`, `overkill=25`; FCT mostra dano de HP efetivo 10, com tooltip/log opcional para nominal. Não curar por overkill. Barreira absorvida tem evento próprio. Balanceamento de DPS e tempo para matar exige simulação, não está provado por estes exemplos.

## 4. HUD — Flutter/Flame

### 4.1 Layout responsivo

HUD superior esquerda dentro de SafeArea: retrato pixel 48×48/64×64, nome curto, nível, HP vermelho acima e MP azul abaixo. Bastião recebe barra auxiliar de vigor âmbar. Barras mostram números atuais/máximos e ícone/abreviação, sem depender só de cor. XP em faixa discreta separada, não confundida com MP.

| Elemento | Comportamento |
|---|---|
| HP/MP/VG | Estado confirmado, animação de transição curta sem ocultar valor numérico atual |
| Consumíveis | Quantidade confirmada, cooldown radial/numérico, tooltip do atalho |
| Habilidades | Custo e recurso explícitos; visual distinto para cooldown, recurso insuficiente e bloqueio de estado |
| Alvo/boss | Barra legível e fases sinalizadas, sem cobrir telegraph |
| Conexão online | Pending/reconciliação visíveis; não afirmar uso aprovado antes da resposta |
| Layout pequeno | Compactar nome/retrato e distribuir botões fora da área principal de combate; preservar texto/controles |

Flame renderiza mundo/VFX; widgets Flutter em overlays representam menus e HUD acessível. Rebuild somente em mudanças relevantes de estado, não criar widgets por inimigo a cada frame. Arte usa nearest e escala inteira quando possível; texto pode renderizar em resolução própria para legibilidade.

### 4.2 Floating Combat Text — FCT

| Evento | Cor / rótulo | Apresentação |
|---|---|---|
| Físico normal | Branco #F4F1E8 / amarelo #E8C66A | Número sobe do alvo por 600 ms |
| Crítico | Vermelho brilhante #FF7770 + marca CRIT | Fonte até 1,25×; pequeno tremor opcional |
| Arcano/elemental | Azul #69A9F3 / roxo #BD8BEA + ícone de tipo | Uma cor primária e símbolo, sem confundir com MP |
| Cura | Verde #79C96B + sinal + | Mostrar HP efetivamente restaurado |
| Esquiva/ataque sem contato | MISS | Sem valor de dano; distingue esquiva de alvo inalcançável no log |
| Imunidade explícita | RESIST | 0 dano; resistência parcial mantém número e ícone, não declara falha |
| Barreira | ABSORB com ícone de escudo | Quantidade absorvida separada do dano no HP |

FCT usa eventos de servidor com `event_id`, `action_id`, alvo, tipo, valor, crítico e tick. Deduplicar por encontro/evento; replay de resposta não recria números. Cliente pode prever movimento/animação, mas não confirma crítico/dano/loot online antes do resultado. Correções atualizam barras sem emitir um segundo acerto.

Limite inicial de 32 FCT ativos; agrupar ticks de DoT do mesmo alvo/tipo em janela de 200 ms. Crítico/cura não entram em grupo de dano comum. Reutilizar componentes e remover após expiração; posições seguem âncora do alvo e deslocamento legível. No modo reduzido, desativar tremor e usar tamanho estável. Combate pode ser lido no log acessível sem narrar cada número em alta frequência.

### 4.3 Screen shake e flash

“2 frames” significa **33 ms na referência de 60 FPS**, registrado como duração temporal; não contar frames renderizados, para não variar em máquinas lentas/rápidas. Shake máximo proposto de 2 pixels lógicos, só em pesado recebido ou crítico de boss confirmado; sem acumular amplitudes, cooldown visual de 100 ms. Não mover hitboxes/UI; só offset transitório de câmera.

Flash branco do alvo por 33 ms em crítico válido; alpha limitado para não apagar silhueta. Recursos de efeitos reduzidos desativam shake/flash; áudio e rótulo mantêm feedback. Sem tela inteira estroboscópica. Haptic opcional, configurável e separado da lógica de dano.

## 5. Autoridade, tempo e eventos

Online: servidor Python calcula recursos, colisões de combate, críticos, morte, progressão e recompensas. Simulação de referência 20 Hz, independente da renderização a 60 FPS; timers em ms com resolução de passo declarada. Animação não concede dano. Offline usa adapter local e os mesmos vetores, sem promessa anticheat.

Por comando: validar ownership/versão → validar custo/ação → executar no passo → resolver defesa/barreira/HP → gerar vigor e estados → registrar morte/loot quando aplicável → commit de estado/eventos/resposta. Eventos com IDs estáveis e sequência; efeito confirmado uma vez. Não usar relógio/FPS do cliente para acelerar cooldown, regeneração ou duração.

## 6. Critérios de implementação e testes

- Vetores físicos/mágicos/críticos coincidem entre Dart e Python, incluindo floor e limites.
- Cooldown, recurso cheio, custo insuficiente, retry e duas requisições simultâneas não gastam itens duas vezes.
- Vigor não se gera por DoT, dano próprio ou múltiplos alvos do mesmo ataque; reserva converge corretamente fora de combate.
- Promoção só afeta expedição revisitada e conserva snapshot; elite promovido recebe categoria de recompensa correta.
- Eclipse exige desbloqueio e novo personagem; morte persistente não é desfeita por load/reconexão.
- HUD/FCT deduplicam, mostram HP real e preservam telegraphs; metas de frame/memória são medidas em release.
- Shake/flash usam tempo, funcionam com efeitos reduzidos e não alteram colisão.
- Nenhum dano/estado hostil atinge o jogador no hub seguro.

Documento entrega regras e contratos, não runtime ou teste de carga. Custos, preços, multiplicadores e taxas de promoção são baseline experimental a validar no protótipo.
