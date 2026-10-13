# Sprint 1 — Classes e atributos

**Projeto:** Echoes of Aethel · **Versão:** 1.0 · **Estado:** especificação para protótipo; valores sujeitos a playtest.

Este módulo é a referência canônica para atributos, dimensões dos sprites jogáveis e árvores de classe. Substitui o desenho preliminar de três ramos e seis atributos. Combate ARPG em tempo real com apresentação 16-bit; as fórmulas são originais, inspiradas na simplicidade dos RPGs de console, sem alegar reproduzir a matemática de Final Fantasy VI.

Regras de combate/recursos são complementadas pela [Sprint 5](05_combate_e_hud.md): vigor do Bastião, regeneração somente fora de combate e crítico até 2×.

## 1. Sistema de atributos

| Atributo | Sigla | Função |
|---|---|---|
| Força | STR | Escala de golpes pesados, maças e machados |
| Destreza | DEX | Escala de adagas, precisão ofensiva e chance de crítico |
| Inteligência | INT | Escala arcana, veneno de Éter e invocações |
| Fé | FTH | Escala sagrada, autocura e escudos rituais |
| Vitalidade | VIT | Vida máxima e parcela natural da defesa física |

Existem exatamente cinco atributos distribuíveis. Sabedoria e Vontade permanecem termos narrativos da Invocadora, cuja mecânica usa INT/FTH. Éter é o recurso exibido como MP. Precisão é derivada, sem sexto atributo ou rolagem de erro no protótipo: alcance, direção, colisão e esquiva determinam acerto.

### 1.1 Valores iniciais e crescimento

| Classe | STR | DEX | INT | FTH | VIT | Primário / secundário | HP / MP iniciais |
|---|---:|---:|---:|---:|---:|---|---|
| Tecelão | 4 | 6 | 14 | 8 | 8 | INT / FTH | 124 / 102 |
| Sentinela | 8 | 14 | 4 | 4 | 10 | DEX / STR | 140 / 54 |
| Bastião | 12 | 6 | 3 | 5 | 14 | STR / VIT | 172 / 52 |
| Invocadora | 4 | 6 | 12 | 10 | 8 | INT / FTH | 124 / 98 |
| Inquisidor | 10 | 4 | 4 | 14 | 8 | FTH / STR | 124 / 74 |

Cada classe começa no nível 1 com 40 pontos totais. A cada nível: 3 pontos distribuíveis, sem crescimento oculto nos cinco atributos. Limite experimental: nível 30 e atributo efetivo entre 1 e 99, incluindo equipamento. Respec no hub devolve somente os pontos distribuídos; não altera a base da classe nem escolhas narrativas.

### 1.2 Estatísticas derivadas

Usar inteiros e `floor` (arredondar para baixo), calculando novamente após mudança de equipamento/atributo. `L` é nível; armadura e ward são valores não negativos do equipamento.

| Estatística | Fórmula |
|---|---|
| HP máximo | `60 + 8×VIT + 6×(L−1)` |
| MP máximo | `30 + 4×INT + 2×FTH + 3×(L−1)` |
| Defesa física, DEF | `armadura + floor(VIT/2) + 2×(L−1)` |
| Defesa mágica, MDEF | `ward + floor((INT+FTH)/3) + (L−1)` |
| Chance de crítico | `min(25%, 5% + 0,25%×DEX + bonus_crit)` |
| Multiplicador crítico | `min(2,0; 1,5 + bonus_multiplicador)`, somente dano direto |
| Regeneração de MP | `1 + floor(INT/10)` MP/s somente fora de combate, conforme Sprint 5 |

Não há regeneração natural de HP em combate. Cura no hub restaura HP/MP. Equipamento que diminui máximos limita o valor atual ao novo máximo, sem curar ao ser trocado. Velocidade de movimento e esquiva são comuns às classes no baseline; DEX não torna telegraphs inviáveis para o Bastião.

### 1.3 Dano e defesa — cálculo determinístico

Cada habilidade informa poder `P`, atributo de escala `A`, tipo e eventual bônus específico. Arma traz `W`, seu poder ofensivo compatível com o ataque; magias e invocações usam foco, não somam uma segunda arma. Para ataques físicos leves, `A=DEX`; pesados, `A=STR`; arcanos/veneno/invocação, `A=INT`; sagrados, `A=FTH`.

1. `base = P + W + 2×A + 2×(L−1)`.
2. `ofensivo = floor(base × (1 + bonus_dano))`, com bônus aditivos limitados entre 0 e 100%.
3. `critico = floor(ofensivo × multiplicador_critico)` se crítico válido; caso contrário, `ofensivo`. Apenas um sorteio por acerto; ticks de DoT não criticam.
4. `mitigado = floor(critico × 100 / (100 + defesa))`. Usar DEF em físico e MDEF em arcano, gelo, fogo, sagrado e veneno.
5. `resistido = floor(mitigado × (100 − resistencia)/100)`, resistência por tipo limitada a −50%…75%.
6. `final = floor(resistido × (1 − reducao_temporaria))`, redução total limitada a 60%. Se o ataque acertou e não há invulnerabilidade explícita, dano mínimo de 1; imunidade/esquiva válida produz 0.

Escudos absorvem o dano final antes do HP, até sua capacidade, sem nova mitigação. Redução temporária soma guarda e passivas até o teto; não multiplica fatores ilimitadamente. Dano misto é dividido em componentes antes da defesa; o ataque inteiro só faz uma rolagem de crítico. O Inquisidor alterna ataques físicos e sagrados; não precisa de dano misto no baseline.

**Exemplo:** Bastião nível 1, STR 12, arma W=8, Golpe P=10: base 42. Contra DEF=20, sem outros efeitos: `floor(4200/120)=35 HP`. Crítico: 63 antes da defesa, resultando em 52 HP. Guarda de 40% sobre o golpe normal: `floor(35×0,6)=21 HP`.

### 1.4 Cura, barreira e dano contínuo

- Cura: `floor((P + 2×FTH) × (1 + bonus_cura))`, bônus limitado a 50%; não critica, não usa arma/defesa e não excede HP máximo.
- Barreira: capacidade `P + 2×A`, conforme habilidade; não acumula consigo mesma. Reaplicar substitui capacidade/duração, sem somar o restante.
- DoT: aplicar a fórmula de dano sem crítico com `P_tick`, uma vez por segundo, durante o número de ticks declarado. Mesmo efeito e mesmo lançador renovam duração sem somar stacks. Lançadores diferentes podem coexistir até três fontes por alvo; escolher as três maiores potências, sem multiplicar procs.
- Invocações usam snapshot de nível/INT/foco do herói no lançamento. Não herdam crítico, passivas de ataque básico ou cura por dano; têm seus próprios eventos e limite de entidades.

## 2. Árvore e regras compartilhadas

Cada classe tem **quatro habilidades ativas e duas passivas**. A1 é o ataque inicial gratuito; A2 é liberada no nível 2, A3 no 4, A4 no 6. P1 abre no 3 após A2; P2 no 5 após A3. A2–A4/P1–P2 custam um ponto cada. Um ponto por nível desde o 2 permite comprar os cinco nós até o 6. A4 requer A3. Sem ranks nesta versão; todos os nós comprados funcionam, sem pré-condição narrativa.

Cooldown começa na execução validada, junto do gasto do recurso indicado. Sem alvo válido, recurso insuficiente, animação bloqueada ou cooldown ativo: comando recusado sem custo. Custos não são debitados de novo por retry. Repetir o mesmo comando não repete dano ou recompensa. Passivas custam 0 MP; cooldown “—” significa efeito contínuo, e cooldown explícito limita o disparo.

Esquiva e interação são ações universais fora da árvore. Esquiva/dash revisada pela [Sprint 6](06_arquitetura_software.md): 25 stamina, cooldown 1,2 s, duração 0,2 s, sem atravessar paredes; stamina é separada de MP/vigor e invulnerabilidade não é implícita. Ataques gratuitos têm cooldown que também limita sua cadência. Diálogo offline pausa combate e timers juntos.

## 3. Mago — Tecelão de Runas

**ID:** `rune_weaver` · **Primário:** INT · **Secundário:** FTH. Combina controle e área com barreira, exigindo distância e gestão de MP.

**Sprite:** 32×32, quatro direções; manto violeta #A17AC4, sombra azul #29334E, runas marfim #DDD5BD. Cajado com ponta clara e silhueta estreita. Origem nos pés; magia tem sprite próprio e não aumenta o colisor.

| Nó / habilidade | MP | CD | Tipo / escala | Efeito e parâmetros iniciais |
|---|---:|---|---|---|
| A1 — Projétil Rúnico | 0 | 0,8 s | Arcano / INT | P=6; projétil acerta primeiro alvo/obstáculo |
| A2 — Lança de Geada | 12 | 5 s | Gelo / INT | P=18; lentidão 25% por 2 s; boss recebe 10% |
| A3 — Círculo de Brasas | 20 | 8 s | Fogo / INT | P=12 em área; queimadura P_tick=2 por 3 ticks |
| A4 — Nó de Éter | 18 | 12 s | Utilidade / INT | Barreira P=20 por até 4 s |
| P1 — Fratura Rúnica | 0 | 3 s | Modificador / arcano | +15% no próximo dano direto contra alvo sob sua lentidão; sem autociclo |
| P2 — Memória do Selo | 0 | — | Recurso / sem dano | +1 MP/s à regeneração enquanto sua barreira estiver ativa; somente fora de combate conforme Sprint 5 |

**Vael:** “Minha mão desenhou a primeira runa que sua ordem chamou de descoberta.”

**Herói:** “Então aprendi o bastante para apagar sua assinatura.”

**Reação:** o retrato passa de espanto a determinação; o cajado desfaz a runa projetada por Vael. É ação narrativa, sem dano ou vantagem gratuita antes da luta.

## 4. Ladina — Sentinela das Sombras

**ID:** `shadow_sentinel` · **Primário:** DEX · **Secundário:** STR. Combate curto, armadilhas e reposicionamento; ocultação não elimina automaticamente o conhecimento da IA.

**Sprite:** 32×32; capa noite #29334E, prata #AEB8C4, detalhe sangue #A94750. Duas adagas curtas e capuz recortado. Armas assimétricas exigem sprites próprios, sem espelhamento automático.

| Nó / habilidade | MP | CD | Tipo / escala | Efeito e parâmetros iniciais |
|---|---:|---|---|---|
| A1 — Corte de Adaga | 0 | 0,5 s | Físico / DEX | P=4; uma execução, um acerto por alvo |
| A2 — Passo Velado | 10 | 10 s | Utilidade / sem dano | Ocultação por 2 s; quebra ao atacar/sofrer dano; mantém posição conhecida |
| A3 — Armadilha de Agulhas | 14 | 7 s | Físico / DEX | P=10; sangramento P_tick=2 por 3 ticks; uma armadilha ativa, expira em 8 s |
| A4 — Ruptura Tendínea | 18 | 6 s | Físico / DEX | P=22; lentidão 25% por 2 s, 10% em boss |
| P1 — Última Sombra | 0 | 10 s | Modificador / físico | +20% ao primeiro A1 até 2 s após sair de Passo Velado; consumo único |
| P2 — Sangue Frio | 0 | — | Modificador / físico | +10% ao dano direto contra alvo com seu sangramento; ticks não ativam |

**Vael:** “A marca nas suas veias não é uma doença. É minha herança.”

**Heroína:** “Herança se recusa. Começo pelo seu nome.”

**Reação:** a mão que escondia a marca abre sobre a adaga; uma respiração curta substitui o recuo. Ocultação não é acionada pela cutscene.

## 5. Guerreiro — Bastião do Crepúsculo

**ID:** `twilight_bastion` · **Primário:** STR · **Secundário:** VIT. Defesa frontal, golpes comprometidos e controle de aproximação.

**Sprite:** 32×32; armadura azul-ferro #565E65, sombra #29334E, escudo âmbar #B59050. Corpo largo, machado e escudo com silhuetas distintas. Colisor nos pés, sem incorporar largura decorativa do escudo.

| Nó / habilidade | Vigor | CD | Tipo / escala | Efeito e parâmetros iniciais |
|---|---:|---|---|---|
| A1 — Golpe de Machado | 0 | 0,9 s | Físico / STR | P=10; arco frontal curto |
| A2 — Guarda do Crepúsculo | 8 | 6 s | Utilidade / sem dano | Redução frontal 40% por 2 s; costas desprotegidas; gera vigor apenas conforme limites da Sprint 5 |
| A3 — Impacto do Baluarte | 12 | 7 s | Físico / STR | P=18; interrupção de 0,5 s em comuns; bosses não sofrem stun |
| A4 — Fenda de Aço | 20 | 9 s | Físico / STR | P=30; linha curta, antecipação própria de 0,6 s |
| P1 — Postura Firme | 0 | 6 s | Defesa / sem dano | Após primeiro bloqueio de A2, +10 pontos percentuais de redução por 2 s; teto compartilhado 60% |
| P2 — Peso do Juramento | 0 | — | Modificador / físico | +15% em A4 se HP atual ≤50% do máximo; não cobra HP nem beneficia A1 |

**Vael:** “Você fechou os portões; eu fechei os céus. Conhecemos o preço de proteger os nossos.”

**Herói:** “Eu ainda escuto quem ficou do lado de fora. Você só escuta a si mesmo.”

**Reação:** baixa o escudo por um instante, reconhecendo a culpa, e volta à postura de guarda. A defesa mecânica só começa após comando válido do jogador.

## 6. Druida — Invocadora de Espinhos

**ID:** `thorn_summoner` · **Primário:** INT · **Secundário:** FTH. Invocação, veneno e sustentação; sabedoria ancestral é expressão narrativa desses atributos.

**Sprite:** 32×32; musgo #526044, osso #DDD5BD, violeta #A17AC4. Cajado ramificado, cabelo/silhueta orgânica. Golem em 48×48 e fera em 32×32, separados da célula da heroína.

| Nó / habilidade | MP | CD | Tipo / escala | Efeito e parâmetros iniciais |
|---|---:|---|---|---|
| A1 — Espinho Errante | 0 | 0,8 s | Arcano / INT | P=5; projétil único |
| A2 — Golem de Pedra | 22 | 14 s | Físico / INT da invocadora | Dura 12 s; HP=40+4×INT, DEF=20, MDEF=10; golpe P=2, intervalo 1,5 s |
| A3 — Jardim de Pragas | 18 | 8 s | Veneno / INT | Área persiste 3 s e aplica 3 ticks de P_tick=2; reentrada não duplica efeito |
| A4 — Fera Ancestral | 24 | 16 s | Arcano / INT | Dura 10 s; HP=30+3×INT, DEF=10, MDEF=20; golpe P=3, intervalo 1,2 s |
| P1 — Raiz Mineral | 0 | — | Defesa / invocação | Golem recebe +10 DEF; não aumenta defesa da heroína |
| P2 — Elo da Matilha | 0 | — | Recurso / sem dano | +1 MP/s de regeneração enquanto houver invocação viva; bônus único, somente fora de combate |

Máximo um golem e uma fera por heroína. Reinvocar substitui entidade anterior sem explosão ou recompensa. Invocações obedecem colisão, não contam como alvo obrigatório de puzzles e são removidas no reset/morte/transição. `W` da invocação é o foco capturado no lançamento.

**Vael:** “Seus ancestrais não me esqueceram. Fugiam de mim através de você.”

**Heroína:** “E chegaram longe o bastante para me ensinar a dizer não.”

**Reação:** vozes se calam por um segundo; o cajado floresce um espinho branco. Não cria invocação fora do limite de entidades.

## 7. Paladino — Inquisidor da Chama Sagrada

**ID:** `sacred_flame_inquisitor` · **Primário:** FTH · **Secundário:** STR. Maça física, fogo sagrado e autocura; sem imunidade narrativa a corrupção.

**Sprite:** 32×32; marfim #DDD5BD, cobre #B59050, chama azul #73BDE0. Tabardo gasto, maça larga, rosto exposto. Símbolo religioso original, sem copiar emblemas das referências.

| Nó / habilidade | MP | CD | Tipo / escala | Efeito e parâmetros iniciais |
|---|---:|---|---|---|
| A1 — Maça do Exílio | 0 | 0,9 s | Físico / STR | P=8 |
| A2 — Martelo de Cinzas | 12 | 5 s | Sagrado / FTH | P=16; queimadura sagrada P_tick=2 por 3 ticks |
| A3 — Chama de Vigília | 20 | 10 s | Cura / FTH | P=18; autocura imediata; sem dano e sem consumir quando HP cheio |
| A4 — Veredito Solar | 24 | 12 s | Sagrado / FTH | P=28; cone frontal, antecipação própria 0,7 s |
| P1 — Brasa Persistente | 0 | — | Modificador / sagrado | A2 ganha um tick adicional; não adiciona stack |
| P2 — Última Confissão | 0 | — | Modificador / sagrado | +15% em dano direto sagrado contra mortos-vivos; não aumenta cura nem ticks |

**Vael:** “Sua chama e a minha nasceram da mesma fome. O fogo não distingue pai e filho.”

**Herói:** “Então terá de aprender a distinguir escolha de obediência.”

**Reação:** retira o antigo medalhão, mas mantém a chama. A cena não troca equipamento nem concede buff sem atualizar o estado persistente.

## 8. Caixas clássicas de diálogo

Markdown não garante fundos e bordas coloridas em todos os renderizadores. As falas acima continuam acessíveis em texto/tabelas; a UI do jogo aplica o contrato visual abaixo. O bloco HTML é referência de composição, não uma dependência executável do jogo.

| Elemento | Especificação |
|---|---|
| Fundo | Gradiente azul #17213B → #29334E |
| Borda dupla | 1 px marfim #DDD5BD, intervalo 1 px azul, 1 px prata #AEB8C4 |
| Texto | #F4F1E8, sombra preta 1 px; fonte pixel original/licenciada e fallback legível |
| Retrato | 64×64 com expressão; não determina tamanho do sprite de mundo |
| Interação | Avanço manual, leitura imediata opcional, foco visível e escala de texto |

<div style="background:linear-gradient(#17213B,#29334E);border:3px double #DDD5BD;padding:12px;color:#F4F1E8;">
<strong>Vael</strong><br>
“Você é meu sangue. Tome o seu lugar ao meu lado… ou sirva ao ritual.”
</div>

<div style="background:linear-gradient(#17213B,#29334E);border:3px double #DDD5BD;padding:12px;color:#F4F1E8;">
<strong>Bastião do Crepúsculo</strong><br>
“Eu ainda escuto quem ficou do lado de fora. Você só escuta a si mesmo.”
</div>

## 9. Contrato técnico e validação

Uma definição de habilidade contém ID estável, classe, nó, pré-requisitos, resource_type (MP/VG) e custo inteiro, cooldown em milissegundos, tipo, atributo de escala, poder e efeitos com duração/limite. Passivas distinguem efeito contínuo de proc e não são comandos equipáveis. IDs de habilidades podem seguir `classe.a1`…`classe.p2`; textos são separados das regras.

Testes a implementar com o domínio: atributos efetivos limitados; máximos consistentes; exemplo 42→35 e crítico 63→52; MP insuficiente sem execução; cooldown após comando válido; passiva disparada uma vez; reset de invocações; ticks sem crítico; cura sem overflow; escolhas/saves sem duplicação. Nada disso é anunciado como código já existente.

Esta entrega valida editorialmente cinco classes, quatro ativas e duas passivas por classe, bases de 40 pontos e fórmulas de HP/MP. Paridade de DPS, sobrevivência e utilidade exige simulação e playtest; kits lentos/rápidos são deliberadamente distintos, mas os números ainda não provam equilíbrio.

Na primeira fatia, implementar Bastião com A1/A2 e esquiva; adicionar A3/A4/P1/P2 quando houver progressão de nível. Os demais módulos são especificações prontas para implementação, sem alegar jogo executável.
