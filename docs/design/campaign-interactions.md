# Campanha inteira — interações, pessoas e retornos

> PROPOSTA EDITORIAL, não implementação. Gerado de `campaign-interaction-spec.json`; editar o JSON e executar `python tools/validate_campaign_interactions.py --write`.

As doze quests, opções, encontros e gates permanecem os do manifesto canônico. Interações, encenação, papéis táticos e novos nomes abaixo são NOVO/EXPERIMENTAL. Consequências e seus gatilhos exatos permanecem no GDD 04; não antecipar efeitos tardios. Os nomes propostos dos NPCs não são IDs de save.

Meta: cada região oferece uma ação característica, pessoa ou testemunho, descoberta opcional, confronto legível e retorno reconhecível. Igual atenção de design não significa igual duração ou quantidade de puzzles. O Coração não recebe conteúdo de preenchimento.

## Regiões

### Bosque das Névoas — `forest_01`

Quest `act1.spy_hunt`; opções `judge_suspects` / `shelter_villagers`. Confronto: **Garras de Musgo**. Decisão: `before_exit`.

**Identidade:** Investigar confiança sob pressão

**Interação principal:** Ler pegadas, ouvir testemunho e examinar o poço; distinguir indício de certeza.

**Descoberta opcional:** Casa e porão: descobrir como os moradores organizavam o abrigo, com os dois recipientes já propostos.

**Âncora humana:** Maera recebe relatos; os moradores têm razões para temer tanto a estrada quanto a acusação.

**Papel dos encontros:** Lobo pressiona flancos; Cultista força cobertura; Garras combina aproximação e golpe comprometido.

**Leitura e acessibilidade:** Enquadrar poço e bordas; ruído de folhas não mascara o aviso de ataque.

**Retorno e consequência:** Uma família agradece pela água limpa recebida, ou identifica um objeto recolhido da estrada; reação condicional, sem ressuscitar vítimas.

**Ritmo:** Pista e abrigo antes da arena; pausa de retorno depois do confronto.

**Função da recompensa:** Dentes, consumíveis e raiz conforme catálogo existente.

**Hub regional:** Cidadela dos Últimos

**Aceite:** O jogador explica por que os três indícios não provam sozinhos a acusação.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### Pântano dos Lamentantes — `swamp_02`

Quest `act1.desperate_alchemist`; opções `deliver_venom` / `destroy_research`. Confronto: **Matriarca das Águas**. Decisão: `before_exit`.

**Identidade:** Observar e comparar antes de tratar

**Interação principal:** Inspecionar animais tratados e cadernos; seguir margens firmes e sinalizar a passagem de retorno.

**Descoberta opcional:** Ilha lateral de observação com anotações de doses; informação complementar, nunca cura secreta que anule o dilema.

**Âncora humana:** O pesquisador externo mostra melhora real de um paciente e admite não conhecer o efeito tardio.

**Papel dos encontros:** Larvas disputam chão seguro; Afogados avançam lentamente; Harpias obrigam reposicionar; a Matriarca alterna pressão e pausa.

**Leitura e acessibilidade:** Água transitável e lama perigosa distinguíveis por forma; antídoto não é chave aleatória obrigatória.

**Retorno e consequência:** A pessoa tratada mantém nome e contexto; retorno tardio mostra o custo exato da opção, sem punir cura básica.

**Ritmo:** Observação tranquila, travessia ameaçada, laboratório seguro para leitura, confronto.

**Função da recompensa:** Antídotos e materiais aquáticos; nenhum item resolve sozinho a decisão principal.

**Hub regional:** Cidadela dos Últimos

**Aceite:** As duas opções continuam compreensíveis depois da leitura dos cadernos, sem depender de acertar um puzzle.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### O Portal do Julgamento — `temple_03`

Quest `act1.guardian_roots`; opções `seal_sap` / `drain_sap`. Confronto: **Silvano**. Decisão: `after_victory`.

**Identidade:** Compreender o que a proteção alimenta

**Interação principal:** Percorrer ligações entre raízes, casas e selos; observar pares de inscrições antes da arena.

**Descoberta opcional:** Arquivo lateral apresenta manutenção antiga dos selos; pode abrir atalho físico, não terceira solução para a seiva.

**Âncora humana:** Irian comenta a responsabilidade de sustentar abrigo; não exige leitura integral para avançar.

**Papel dos encontros:** Defensores seguram terreno; cães expulsam de posições estáticas; Silvano testa guarda e reposicionamento.

**Leitura e acessibilidade:** Raízes animadas decorativas não mudam o collider sob os pés; fragmento tem assinatura visual distinta do loot.

**Retorno e consequência:** O fragmento obtido prova vitória; casas e vegetação exibem a consequência prevista no marco, não um reset do Bosque.

**Ritmo:** Chegada monumental, investigação breve, arena, escolha depois da vitória, recuperação.

**Função da recompensa:** Fragmento e receita canônicos; recompensa de boss separada da escolha.

**Hub regional:** Cidadela dos Últimos

**Aceite:** A escolha só abre após vencer Silvano; ambas liberam a próxima zona sem retirar o cristal do hub.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### As Dunas de Sal — `dunes_04`

Quest `act2.lost_caravan`; opções `save_supplies` / `save_merchants`. Confronto: **Guerreiro de Vidro**. Decisão: `before_exit`.

**Identidade:** Ler a paisagem e administrar uma travessia

**Interação principal:** Usar ossadas e abrigos como referências; examinar cordas e manifesto da caravana antes de escolher.

**Descoberta opcional:** Rastro de peças desviadas apresenta a entrada da Coroa de Sucata; pode ser ignorado sem bloquear a caravana.

**Âncora humana:** Sobreviventes descrevem seu ofício e dependentes; a carga também representa pessoas que precisam comer.

**Papel dos encontros:** Escorpiões ameaçam o chão; gárgulas interrompem abrigo prolongado; Guerreiro de Vidro expõe-se após avançar.

**Leitura e acessibilidade:** Tempestade reduz decoração, nunca a legibilidade de saída e telegraph; sem barra nova de sede.

**Retorno e consequência:** No acampamento, uma encomenda tem destinatário e uma ausência tem nome; a loja essencial permanece.

**Ritmo:** Travessia aberta, abrigo, decisão, ameaça concentrada, descanso no acampamento.

**Função da recompensa:** Quartzo, resistência e essência solar; suprimentos básicos disponíveis sem dungeon.

**Hub regional:** Acampamento das Dunas

**Aceite:** Há caminho claro de ida e volta mesmo com efeitos reduzidos; a dungeon não vira requisito de equipamento.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### O Vale dos Sacrifícios — `valley_05`

Quest `act2.flame_ritual`; opções `offer_vitality` / `ritual_execution`. Confronto: **Executor de Bronze**. Decisão: `before_exit`.

**Identidade:** Ler uma sentença e assumir um custo

**Interação principal:** Conferir o registro do condenado e seguir inscrições que conectam altar, fonte e tumbas.

**Descoberta opcional:** Mirante de pedreiros revela a função original da ponte e um retorno protegido; não desfaz sentença ou custo.

**Âncora humana:** O prisioneiro é uma pessoa com responsabilidade verificável; não transformar exposição em absolvição automática.

**Papel dos encontros:** Ritualistas preparam zonas; Hienas pressionam flancos; Executor pune permanência no mesmo lugar.

**Leitura e acessibilidade:** Pontes têm espaço de recuo; custo temporário conhecido aparece antes de confirmar.

**Retorno e consequência:** Marcas do altar e notícia das fontes permanecem identificáveis quando o jogador entra na Cripta.

**Ritmo:** Pressão na ravina, pausa de evidência, compromisso, combate e passagem.

**Função da recompensa:** Bronze, runas e chama; nenhum prêmio moral novo sem orçamento aprovado.

**Hub regional:** Acampamento das Dunas

**Aceite:** Custo de vitalidade expira como no GDD, inclusive na morte/abandono do encontro associado; retry não cobra novamente.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### A Cripta de Ossos — `crypt_06`

Quest `act2.ossuary`; opções `free_captives` / `reinforce_seal`. Confronto: **Malignus**. Decisão: `after_victory`.

**Identidade:** Reconhecer pessoas por trás de uma prisão

**Interação principal:** Ouvir nomes e observar correntes reagindo à pressão do selo; investigar antes de decidir o destino dos cativos.

**Descoberta opcional:** Inscrição lateral introduz o Livro dos Nomes Perdidos; todas as provas da quest principal ficam fora da dungeon.

**Âncora humana:** Uma alma reconhece um registro de família; Irian conserva versões contraditórias, sem falar por todas as vítimas.

**Papel dos encontros:** Múmias sustentam grupos; sombras pressionam posicionamento; Malignus combina contenção e janelas de ataque.

**Leitura e acessibilidade:** Caminhos e piso contrastam com ossos decorativos; vozes têm legenda e repetição acessível.

**Retorno e consequência:** O retorno diferencia libertação e prisão mantida pelo estado canônico; não substitui Malignus por Namar.

**Ritmo:** Galerias silenciosas, ataques pontuais, ossuário, boss, escolha e retirada.

**Função da recompensa:** Fragmento e receita do Ossuário; primeira recompensa única independente da dungeon.

**Hub regional:** Acampamento das Dunas

**Aceite:** Recusar Namar não impede fragmento, revelação ou progressão ao ato III.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### Os Esgotos de Éter — `sewers_07`

Quest `act3.purification_valve`; opções `drain_lower` / `reverse_flow`. Confronto: **Mãe das Proles**. Decisão: `before_exit`.

**Identidade:** Entender infraestrutura e quem vive abaixo

**Interação principal:** Inspecionar diagrama e marcas de moradia; usar passagens de manutenção para observar destino do fluxo.

**Descoberta opcional:** Arquivo de reparos revela quem mantinha os canais; abre retorno curto com piso estável.

**Âncora humana:** Uma técnica dos canais tem conhecimentos e vizinhos; fala como trabalhadora, não como painel de instruções.

**Papel dos encontros:** Ratos atacam em aproximações curtas; mutantes seguram passagens; aranhas sinalizam controle de espaço; Mãe das Proles exige priorização.

**Leitura e acessibilidade:** Água é legível em alturas diferentes; o hub nunca recebe dano após a mudança de fluxo.

**Retorno e consequência:** Relatos de moradores e familiares tornam o destino do resíduo concreto, no gatilho já previsto.

**Ritmo:** Leitura hidráulica, travessia, pausa de decisão, ninho, retorno seco.

**Função da recompensa:** Catalisadores e essência tóxica; drenagem não cria fonte de farm por reload.

**Hub regional:** Refúgio Subterrâneo

**Aceite:** O jogador consegue apontar quem fica abaixo/acima antes de decidir; efeitos não bloqueiam o retorno.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### A Fundição Proibida — `foundry_08`

Quest `act3.energy_core`; opções `extract_core` / `stabilize_core`. Confronto: **Autômato Forjador**. Decisão: `before_exit`.

**Identidade:** Separar capacidade de produzir de responsabilidade

**Interação principal:** Observar ciclos de máquinas e medidores; escutar exploradores e verificar o posto de contenção.

**Descoberta opcional:** Oficina de manutenção mostra reparos e reuso; oferece descoberta temática, sem iniciar crafting avançado.

**Âncora humana:** Uma exploradora identifica companheiros pela voz; Bram reage depois por correspondência, não por teletransporte narrativo.

**Papel dos encontros:** Constructos bloqueiam; elementalistas disputam posições; Autômato Forjador tem silhueta e recuperação próprias.

**Leitura e acessibilidade:** Primeira implementação usa piso estático; máquinas móveis só após teste de colisão e rota alternativa.

**Retorno e consequência:** Peças trazidas à oficina ou vozes na máquina correspondem à decisão, com ganho limitado e único.

**Ritmo:** Cadência de máquinas, janela de observação, decisão sob pressão narrada, confronto.

**Função da recompensa:** Ligas, gemas e essência de forja; upgrade do núcleo respeita orçamento e unicidade.

**Hub regional:** Refúgio Subterrâneo

**Aceite:** Extrair/equipar/recarregar não duplica melhoria; estabilizar não prende o personagem com os exploradores.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### O Abismo da Voracidade — `abyss_09`

Quest `act3.last_hunger`; opções `break_bond` / `seal_creature`. Confronto: **Gorgoroth**. Decisão: `after_victory`.

**Identidade:** Ouvir testemunhos e atravessar o horror sem perder orientação

**Interação principal:** Seguir fios visuais de vínculo e identificar túneis habitados; parar em abrigo antes da arena.

**Descoberta opcional:** Registro de uma expedição anterior contextualiza a rede; informação de linhagem obrigatória tem alternativa fora da secundária do Traidor.

**Âncora humana:** Uma vítima descreve a voz de sua filha; o jogo distingue esperança, fato observado e imitação.

**Papel dos encontros:** Parasitas forçam mudança de alvo; abominações definem espaço; Gorgoroth testa leitura de vínculos e retirada.

**Leitura e acessibilidade:** Geometria estável sob superfície orgânica; modo reduzido mantém leitura sem pulsação intensa.

**Retorno e consequência:** Libertados ou memoriais sem nomes aparecem conforme a escolha e o marco do ato III.

**Ritmo:** Opressores corredores curtos, abrigo, boss, escolha pós-vitória e saída limpa.

**Função da recompensa:** Fragmento e Foco Abissal conforme cânone; recompensas sem redução permanente por decisão.

**Hub regional:** Refúgio Subterrâneo

**Aceite:** Vencer Gorgoroth continua sendo vitória real; vínculo/fragmento e decisão sobrevivem a reload.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### Os Jardins Suspensos de Éter — `gardens_10`

Quest `act4.judgment_throne`; opções `grant_pardon` / `condemn_souls`. Confronto: **Arconte de Prata**. Decisão: `before_exit`.

**Identidade:** Julgar uma instituição pela origem de sua energia

**Interação principal:** Ler listas do pedestal e observar muralhas distantes; comparar a proteção prometida com quem a sustenta.

**Descoberta opcional:** Observatório oferece contexto das barreiras e um ponto de vista alternativo; não revela o epílogo.

**Âncora humana:** Registros de responsáveis e almas substituem sermão de um narrador onisciente.

**Papel dos encontros:** Sentinelas controlam linhas; espectros obrigam reposicionar; Arconte expõe a lógica de proteção no confronto.

**Leitura e acessibilidade:** Mármore claro não apaga telegraphs; guardas visuais nas bordas e oclusão resolvida.

**Retorno e consequência:** O Posto comenta refugiados ou preservação das barreiras; efeito completo na Cidadela segue o gatilho existente.

**Ritmo:** Amplitude e silêncio, leitura breve, combate ordenado, decisão e subida.

**Função da recompensa:** Materiais celestiais e essência estelar; buff conhecido limitado ao encontro associado.

**Hub regional:** Posto do Limiar

**Aceite:** Decisão não ameaça o cristal próprio da Cidadela; custo/benefício mecânico aparece sem spoilers futuros.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### A Ponte dos Lamentos — `bridge_11`

Quest `act4.last_gate`; opções `break_seal` / `channel_body`. Confronto: **Guardião dos Portões**. Decisão: `before_exit`.

**Identidade:** Preparar uma última passagem e aceitar compromisso

**Interação principal:** Observar fissuras, selo e sinais da escolta; confirmar recursos antes da travessia final.

**Descoberta opcional:** Torre lateral permite rever registros de viagem e ouvir Maera; sem novo desvio longo de loot.

**Âncora humana:** A mensagem de Maera reconhece a escolta e suas escolhas sem tratar o herói como soma de karma.

**Papel dos encontros:** Guardas de Vael e constructos exigem alternância de alvo; Guardião deixa espaço legível para esquiva.

**Leitura e acessibilidade:** Sem queda instantânea por câmera; ruptura narrativa não remove piso sob o herói nem impede retry seguro.

**Retorno e consequência:** A vigília ou a escolta marcada aparece no marco previsto; entrada final não reaplica custo de MP.

**Ritmo:** Preparação calma, travessia tensa, confronto, decisão de selo, soleira.

**Função da recompensa:** Runas e essência de limiar; não exigir farm de dungeon para enfrentar Vael.

**Hub regional:** Posto do Limiar

**Aceite:** O jogador pode se preparar e repetir Vael sem lockout por ponte/custo; a primeira escolha continua gravada.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

### O Coração de Éter — `heart_12`

Quest `act4.heart_fate`; opções `restore_order` / `embrace_ascension`. Confronto: **Vael**. Decisão: `after_victory`.

**Identidade:** Confrontar uma ideologia e escolher o destino do mundo

**Interação principal:** Revisar provas de linhagem já observadas e enfrentar Vael antes de qualquer opção final.

**Descoberta opcional:** Sem dungeon, sala de farm ou nova tarefa obrigatória; revisão opcional de registros preserva o ritmo final.

**Âncora humana:** Vael tenta apropriar-se das escolhas; testemunhos anteriores contradizem seu controle absoluto.

**Papel dos encontros:** Fases propostas para medir domínio das ações aprendidas; nenhuma fase exige habilidade de outra classe.

**Leitura e acessibilidade:** Arena e borda estáveis; eclipse e música não escondem alvos; apresentação pulável restaura controle.

**Retorno e consequência:** Epílogos por regiões e pessoas mostram ganhos reais e custos canônicos; não apagar os nomes que o jogador preservou sem explicar o evento.

**Ritmo:** Revelação, confronto, silêncio pós-vitória, decisão, epílogos com tempo de leitura.

**Função da recompensa:** Fragmento de Vael e recompensa final única; campanha encerrada sem farm implícito.

**Hub regional:** Posto do Limiar

**Aceite:** restore_order e embrace_ascension só aparecem após vitória; pular/assistir epílogo produz o mesmo save.

**Autoridade das consequências:** docs/GDD/04_mapas_e_quests.md; preserve its exact trigger; do not expose future effects

## Hubs com vida própria

### Cidadela dos Últimos — `citadel`

Irian, Bram, Lyra, Sera, Maera (existentes)

Conselho distribui relatos; nenhum NPC precisa repetir toda a exposição.

- Irian: comparar relatos observados e registrar ausentes.
- Bram: inspecionar equipamento; oficina continua mesmo sem aprendiz.
- Lyra: suprimentos e relatos de tratamento.
- Sera: comércio e pistas de caravanas.
- Maera: orientar saída e reconhecer retorno.

**Memória regional:** Fatos específicos prevalecem sobre afinidade agregada; segurança e serviços nunca desaparecem.

Sempre seguros; preparação, save, inventário e loja essencial preservados.

### Acampamento das Dunas — `dunes_camp`

Tessa, intendente de caravana (NOVO/EXPERIMENTAL; nome e ID não aprovados)

Conta suprimentos por famílias, conhece a estrada e contesta simplificações de Sera sem substituí-la.

- Consultar quadro de carga/desaparecidos conforme fatos observados.
- Comprar preparação básica por fornecedor local.
- Ouvir sobrevivente e conhecer a pista de Gorvak, com opção de recusar.

**Memória regional:** Carga ou sobreviventes da caravana alteram falas e objetos; poço secundário e dungeon não decidem a quest principal.

Sempre seguros; preparação, save, inventário e loja essencial preservados.

### Refúgio Subterrâneo — `underground_refuge`

Dara, técnica dos canais; Eron, responsável de provisões (NOVO/EXPERIMENTAL; IDs pendentes)

Dara dá rosto ao trabalho hidráulico já citado no GDD; Eron cuida de pessoas concretas, sem virar apenas vendedor.

- Ler mapa hidráulico com legenda acessível.
- Ouvir versões das comunidades superiores e inferiores.
- Receber relatos da Fundição e notícia verificável do Traidor.

**Memória regional:** Fluxo, contenção e vítimas do Abismo têm retornos locais distintos; plataforma do hub permanece seca e segura.

Sempre seguros; preparação, save, inventário e loja essencial preservados.

### Posto do Limiar — `liminal_post`

Vera, guardiã dos registros e provisões (NOVO/EXPERIMENTAL; ID pendente)

Apresenta provas verificáveis e admite lacunas; não substitui Irian nem fala de eventos ainda não observados.

- Rever evidências de linhagem sem depender da palavra de Vael.
- Preparar combate e verificar recursos.
- Receber comunicação de Maera e rever fatos regionais conhecidos.

**Memória regional:** Refugiados e escolta refletem flags específicas; posto não antecipa final e preserva preparação/retry.

Sempre seguros; preparação, save, inventário e loja essencial preservados.

## Duas dungeons completas em proposta

### A Coroa de Sucata

Entrada: `dunes_04`. Subchefe: **Gorvak, Rei das Cinzas**. Referência editorial: `proposal.dungeon.ash_crown`; não usar como ID de runtime.

**Identidade:** Cerco e logística: entender uma fortaleza construída por quem precisa sobreviver.

**Descoberta:** Sera menciona caravanas; peças e marcas de carga na rota opcional confirmam o rumor sem exigir aceitar a missão.

1. Examinar a entrada e o aviso de risco; voltar livremente.
2. Observar sinais entre vigias goblins e defensores orcs da Coligação das Cinzas.
3. Escolher caminho de combate frontal ou desvio de manutenção; ambos chegam ao mesmo confronto e não são nova escolha moral.
4. Desativar um engenho opcional com interação legível; confirmação persiste e não duplica material.
5. Inspecionar carga e testemunho de sobrevivente; registrar o conflito por água sem declarar um povo maligno.
6. Abrir retirada antes da arena.
7. Enfrentar Gorvak: marreta anunciada, recuperação vulnerável e reforços limitados por fase; sem geometria destruída que prenda saída.
8. Confirmar resultado e recompensa; devolver à âncora segura.
9. Retornar à conversa de Sera para reconhecimento, sem rolar loot novamente.

**Resposta do jogador:** Usar cobertura sinalizada, reposicionar antes da marreta e aproveitar recuperação; reforços têm limite definido em catálogo antes do runtime.

**Resultado narrativo:** Sera reconhece o destino da carga e sobreviventes observados; conclusão não reverte save_supplies/save_merchants.

**Recompensa:** Tabela de subchefe existente; recompensa adicional de primeira quest só após catálogo e simulação. Não prometer dois equipamentos garantidos por ambiguidade.

**Reentrada:** Primeira versão sem reset/farm; retorno de exploração sem recompensas duplicadas. Repetição futura exige encounter_epoch e regra aprovada.

**Entrada e saída:** Bilhete persistido, destino/âncora validados; falha de load recupera origem; morte segue checkpoint seguro sem perder fatos confirmados.

**Aceites:**

- Pode concluir campanha sem entrar.
- Pode sair antes do boss.
- Bolsa cheia preserva recompensa na reserva.
- Vitória antes da aceitação é reconhecida sem repetir boss/loot.
- Gorvak não concede fragmento, carga ritual nem gate obrigatório.

### O Livro dos Nomes Perdidos

Entrada: `crypt_06`. Subchefe: **Namar, o Rei sem Nome**. Referência editorial: `proposal.dungeon.lost_names`; não usar como ID de runtime.

**Identidade:** Memória como prisão: reconstruir testemunhos sem depender de reconhecer som ou cor.

**Descoberta:** Registros de Irian e inscrição lateral da Cripta apontam o ossuário; informação essencial de Malignus permanece na campanha.

1. Ler aviso de perigo e registrar âncora de retorno.
2. Relacionar placas de nomes a registros de famílias; texto e ícones substituem pistas somente sonoras.
3. Atravessar sala de ecos com inimigos que reagem a sinos anunciados.
4. Silenciar um sino opcional por mecanismo acessível; persistir estado, sem puzzle de senha aleatória.
5. Ouvir contradição entre um nome preservado e a pessoa que o carregava; não revelar outra origem para Vael.
6. Abrir retirada antes da câmara.
7. Enfrentar Namar: comandos de invocação limitados, círculos de silêncio visíveis e janela após cada comando; básico e esquiva continuam utilizáveis.
8. Confirmar derrota e registro dos nomes; recompensa uma vez.
9. Voltar a Irian para microcena pulável de memória, sem transferir a ele a autoridade do save.

**Resposta do jogador:** Priorizar invocações relevantes, sair do círculo e punir o intervalo de comando; silêncio não pode inutilizar todas as respostas de uma classe.

**Resultado narrativo:** Nomes recuperados ganham lugar no arquivo; não substituem o dilema free_captives/reinforce_seal e não libertam retroativamente os cativos de Malignus.

**Recompensa:** Categoria de subchefe existente, orçamento de mesmo ato. A recompensa informa função/raridade conhecida e nunca vende certeza não garantida.

**Reentrada:** Primeira versão sem reset/farm; nova campanha tem identidade própria. Reentrada não repete vitória, item ou XP.

**Entrada e saída:** Mesmo contrato transacional da Coroa; nome de cena não é ID persistido; nenhum checkpoint criado automaticamente.

**Aceites:**

- Malignus e a campanha funcionam sem Namar.
- Todas as pistas têm alternativa textual.
- Silêncio mantém pelo menos resposta ofensiva e evasiva válida por classe.
- Pular/assistir retorno dá o mesmo estado.
- Namar não concede fragmento nem altera a carga ritual.

## Secundárias narrativas

### O Julgamento do Cultista

Local proposto: `forest_01`. Referência editorial: `proposal.side.cultist_judgment`. Sem carga ritual.

Pessoas distintas dos acusados da aldeia; depoimento sobre crianças e rota de resgate.

Investigar um ponto lateral seguro, confrontar versões e confirmar opção; não reutilizar a flag da aldeia.

- Poupar: resgate possível e contaminação posterior.
- Executar: vetor interrompido, crianças sem resgate e maldição alimentada.

**Retorno:** Maera/Irian

### A Água da Vida

Local proposto: `valley_05`. Referência editorial: `proposal.side.water_of_life`. Sem carga ritual.

Poço lateral antes do altar principal; soldados melhoram, inscrição mostra ligação ao selo.

Inspecionar poço e falar com beneficiário; escolha separada da vitalidade do mapa 05.

- Canalizar: cura e fanatismo.
- Selar e inutilizar saída: barreira preservada, cura perdida.

**Retorno:** Acampamento das Dunas; relato posterior de Lyra

### A Aliança com o Traidor

Local proposto: `foundry_08`. Referência editorial: `proposal.side.traitor_alliance`. Sem carga ritual.

Prisioneiro ligado à rede de almas numa sala lateral; um registro verifica parte da oferta.

Investigar e decidir sem bloquear o núcleo ou colocar linhagem obrigatória somente nesta missão.

- Libertar: informação e fortalecimento da rede.
- Matar: rede encerrada e onda que altera encontros futuros, sem reduzir XP/loot permanentemente.

**Retorno:** Refúgio Subterrâneo

## Ainda não fechado

A validação confirma cobertura estrutural e preservação do manifesto. Não aprova novos nomes, balanceamento, planta, assets, código ou gameplay. Gates restantes:

- Editorial approval of new names/interactions
- Metric layouts and camera test
- Item catalog and economy simulation
- Unity implementation
- Assets rigs animation audio
- Windows build and human playtest
