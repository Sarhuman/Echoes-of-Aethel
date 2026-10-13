# Sprint 7 / Módulo 08 — Diálogos do hub e degradação moral

**Projeto:** Echoes of Aethel · **Versão:** 1.0 · **Estado:** especificação narrativa e de dados; scripts de runtime pendentes.

> **Spoilers de produção.** A Cidadela parece cada vez mais protegida enquanto perde a capacidade de cuidar de seus habitantes. A corrupção é invisível como número, mas visível em suas consequências.

Este módulo aprofunda os [mapas e decisões](04_mapas_e_quests.md), o [roteiro](07_roteiro_e_dialogos.md) e a [arquitetura Godot](06_arquitetura_software.md). Não altera as 24 opções ou cria dano no hub. Sprint 7 é o nome desta entrega; módulo 08 é o caminho no GDD, pois o roteiro já ocupa o 07.

## 1. Identidades e continuidade

| Papel | Nome canônico atual | ID estável para código | Continuidade |
|---|---|---|---|
| Ferreiro | **Bram** | `npc.blacksmith` | Substitui o nome de apresentação Oren; é o mesmo personagem, não outro ferreiro |
| Alquimista | **Lyra** | `npc.alchemist` | Substitui Lysa; mantém atendimento e quests já associados à alquimista |
| Ancião do Conselho / líder dos refugiados | **Irian** | `npc.elder` | Continua responsável pelo arquivo e passa a representar refugiados no conselho |
| Guarda do portão | Maera | `npc.gatekeeper` | Identidade preservada |
| Mercadora / abastecimento | Sera | `npc.merchant` | Não é substituída pelo líder dos refugiados |

Nomes de exibição não são chaves de save. Os documentos ativos são reconciliados para Bram/Lyra; referências originais preservadas não são reescritas. IDs legados de linha `hub.oren.*` / `hub.lysa.*`, se usados num protótipo externo, precisam de aliases/migração para `hub.bram.*` / `hub.lyra.*`. Nenhum save executável existente foi identificado nesta entrega.

## 2. Medidor e projeção visual

### 2.1 Corrupção não é relógio de espera

`ritual_charge` aumenta em 1 por decisão principal confirmada, até 12, conforme módulo 04. `mercy` e `judgment` são afinidades separadas. Ambas alimentam o ritual: não existe uma rota sem custo. O contador não cresce por tempo offline, reload ou repetição de conversa.

**Estágio do hub** é derivado de atos concluídos e do marco pré-final; **flags observadas** aplicam consequências específicas. Carga ritual sozinha não revela um evento futuro. Maioria de judgment torna a atmosfera mais disciplinadora; maioria de mercy favorece culto à cura/sacrifício. Empate mantém elementos mistos. Não decidir automaticamente que toda pessoa se torna fanática após certo número.

| Estágio | Gate persistido | Pedra, luz e som | Rotina/social |
|---|---|---|---|
| 0 — Chegada | Prólogo | Pedra clara #DDD5BD, sombras #555866, tochas âmbar #DF8846 | Feira pequena, artesãos conversam, passos e martelos regulares |
| 1 — Ato I | Mapa 3 resolvido | Veios roxos discretos #A17AC4, água isolada se contaminada, luz mais fria | Sorrisos escassos, turnos de cuidado, famílias evitam falar dos sintomas |
| 2 — Ato II | Mapa 6 resolvido | Cristais nas juntas, fuligem ocre, tochas reduzidas visualmente | Filas, barracas secundárias fechadas, placas de sentença ou pedidos de auxílio |
| 3 — Ato III | Mapa 9 resolvido | Paredes parcialmente cobertas de cristal #BD8BEA, névoa decorativa baixa | Murmúrios, oficinas militarizadas ou enfermaria tratada como santuário |
| 4 — Pré-final | Mapa 11 resolvido | Veios lilases iluminam a praça; tochas apagadas substituídas por Éter | Silêncio ritual, saudações sincronizadas e culto à proteção |
| Epílogo | Escolha do mapa 12 | Ordem: cristal estável e portões fechados; Ascensão: cidade recomposta | Diálogos finais do módulo 07, com flags de sobreviventes |

Névoa/cristal não obscurecem navegação, foco de UI ou interação. Iluminação ambiente mínima permanece legível. O silêncio é direção sonora, não ausência de feedback operacional. A Cidadela continua com HP/MP/save/inventário acessíveis.

### 2.2 Lojas fechadas e segurança

Fechar uma barraca secundária é consequência visual/narrativa. Nunca fechar o único ponto de compra de poções, cura ou preparação. Se Bram ou Lyra se afastam num evento, assistente assume interface no mesmo local, com preços/regras dos módulos 03/05. Eles não morrem nem viram inimigos atacáveis por essas quests.

Doença, intoxicação e paranoia são encenadas por retrato, fala e rotina, sem debuff de HP no hub. Fanatismo em Vael só pode ser explícito **após o jogador conhecer a conexão de Vael**; antes disso, NPCs veneram “a Ordem”, “o cristal” ou “a proteção”. A rotina degradada não abre combate/crime dentro da Cidadela.

## 3. Diálogos — caixas azuis clássicas

Cada citação é uma fala apresentada com retrato 64×64, fundo azul #17213B→#29334E e borda dupla marfim/prata na engine. Markdown usa citação como fallback; conteúdo fica legível sem desenho de bordas em texto. Texto avança manualmente e pode ser relido no diário.

## 4. Bram — orgulho, paranoia e devoção

**Linha inicial:** `hub.bram.stage0.base`

> **Bram — retrato confiante**  
> “Aço ruim mente no primeiro golpe. O meu avisa antes de quebrar. Se trouxer uma arma de volta, traga também a história dela.”

| Marco | Condição observada | Estado visual | Diálogo / ID |
|---|---|---|---|
| Ato I | `judge_suspects` e memorial registrado | Mandíbula tensa, ferramentas alinhadas demais | `hub.bram.stage1.judgment`: “Vieram lâminas sem dono. Pediram que eu apagasse os nomes. Quem não tem nome nunca pode dizer que foi inocente.” |
| Ato I | `shelter_villagers` e contaminação observada | Olheiras; cobertor do aprendiz na oficina | `hub.bram.stage1.mercy`: “O aprendiz dorme aqui. Diz que escuta a água mesmo longe do poço. Eu bato o martelo mais baixo.” |
| Ato II | `ritual_execution` aplicado | Evita olhar para a rua, guarda escudo junto à bancada | `hub.bram.stage2.judgment`: “A sentença abriu o caminho. Agora qualquer um que chega parece estar escondendo outra porta. Talvez devêssemos julgá-los antes de entrar.” |
| Ato II | Vitalidade oferecida ou resgate de mercadores | Remendos e metais reaproveitados | `hub.bram.stage2.mercy`: “Você pagou com o corpo. Eles querem que eu faça um símbolo disso. Prefiro fazer uma muleta.” |
| Ato III | Núcleo extraído e devastação observada | Fuligem, estandarte de guarda, voz defensiva | `hub.bram.stage3.judgment`: “A máquina obedece. As pessoas perguntam demais. Diga que precisa de mais armas e eu deixo de pensar no resto.” |
| Ato III | Núcleo estabilizado e exploradores presos observados | Mão suspensa sobre a máquina, olhar inquieto | `hub.bram.stage3.mercy`: “Mantivemos a fábrica de pé. À noite, a máquina pede que eu abra uma porta. Não há porta aqui.” |
| Pré-final | ≥2 decisões judgment nos atos II–IV já observadas | Postura rígida, cristal roxo no avental | `hub.bram.stage4.judgment`: “Dúvida é a primeira rachadura. A Ordem nos ensinou a fechar o que não podemos proteger. Vou forjar até ninguém precisar perguntar.” |
| Pré-final | Predomínio mercy, sem gatilho cruel específico | Ajoelha brevemente diante das ferramentas | `hub.bram.stage4.mercy`: “Estão chamando sua dor de bênção. Se nos salvou sofrendo, vão pedir que todos sofram também.” |

A linha de vitalidade só aparece com `offer_vitality`; se o gatilho for apenas resgate de mercadores, usar `hub.bram.stage2.mercy_caravan`: “Eles voltaram vivos. Conserto as carroças devagar; não quero que confundam uma roda nova com uma estrada segura.”

**Fanatismo explícito, depois da revelação:** `hub.bram.revealed.judgment`, exige stage4 + revelação observada + gatilho cruel.

> **Bram — retrato sereno demais**  
> “Vael nos deu uma regra quando só tínhamos medo. Se o senhor da Ordem errou, talvez tenha sido por hesitar. Eu não vou hesitar por ele.”

É fala de Bram influenciado, não verdade do narrador. Ele continua atendendo; serviço não exige concordar com o culto. Retorno opcional para preparação antes do encontro pode mostrar essa variante; jogador que segue diretamente recebe eco equivalente no epílogo, sem viagem obrigatória.

## 5. Lyra — salvação experimental e dependência de Éter

**Linha inicial:** `hub.lyra.stage0.base`

> **Lyra — retrato atento**  
> “Uma cura precisa funcionar amanhã também. Se eu lhe prometer mais do que isso, peça para ver minhas anotações.”

O percurso de dependência requer **`deliver_venom` e efeito do soro observado**. Ingredientes foram entregues ao pesquisador externo; Lyra recebe amostras e registros como parte do mesmo evento, não substitui retroativamente a identidade do pesquisador da quest. O Éter alivia o cansaço por pouco tempo; repetição e paranoia conduzem dependência. Não apresentar intoxicação como atributo divertido ou condição universal da classe.

| Marco | Rota com soro/ingredientes tóxicos | Estado visual | Rota sem entregar veneno |
|---|---|---|---|
| Ato I | `hub.lyra.stage1.serum`: “Os primeiros voltaram a respirar. Guardei uma amostra. Preciso saber por que melhora tudo por alguns minutos.” | Dedos tingidos de violeta; amostra selada na mesa | `hub.lyra.stage1.no_serum`: “Destruíram os lotes. A fila continuou. Quero registrar as duas coisas.” |
| Ato II | `hub.lyra.stage2.serum`: “Só uso o resíduo para terminar o turno. Não é tratamento. Eu sei a diferença. Sei… quando durmo.” | Tremor fino, frasco oculto sob caderno | `hub.lyra.stage2.no_serum`: “Uma família esqueceu o rosto de quem perdeu. Dei papel a ela. Remédio nenhum escreve por nós.” |
| Ato III | `hub.lyra.stage3.serum`: “Se o frasco está longe, escuto todos os pedidos ao mesmo tempo. Perto dele, consigo escolher um. Isso ainda é ajudar, não é?” | Olhos brilhantes; repete a limpeza de uma mesa já limpa | `hub.lyra.stage3.no_serum`: “Fechamos as janelas por causa da névoa. Agora preciso abri-las por causa do silêncio.” |
| Pré-final | `hub.lyra.stage4.serum`: “Dizem que o Éter quer nos refazer. Talvez a parte que resiste seja justamente a parte que sente dor.” | Retrato calmo e exausto, marca no pulso, anotações curtas | `hub.lyra.stage4.no_serum`: “Chamam a proteção de milagre. Continuo contando quem precisou dela e quem ficou fora.” |

Fala de família esquecida exige `free_captives` observado; sem ele, usar `hub.lyra.stage2.no_serum_fallback`: “Prometi registrar cada paciente. Começaram a me pedir que registre apenas os que melhoraram.” Assim, rota sem soro não inventa outra decisão.

> **Lyra — `hub.lyra.revealed.serum`, após conhecer Vael**  
> “Se ele conhece a matéria que nos fez adoecer, talvez conheça a que pode nos devolver. Eu sei o que está pensando. É o mesmo que eu pensava antes do primeiro frasco.”

Não há cura automática dessa dependência por uma conversa; registro persiste no epílogo. Assistente limpa/padroniza frascos vendidos ao jogador: compras continuam usando templates validados, sem item venenoso surpresa no único serviço essencial.

## 6. Irian — Ancião, arquivo e refugiados

**Linha inicial:** `hub.elder.stage0.base`

> **Irian — retrato cansado, firme**  
> “Uma cidade não começa no portão. Começa quando alguém decide guardar o nome de quem ainda está lá fora.”

| Marco | Condição | Visual | Diálogo / ID |
|---|---|---|---|
| Ato I | Mortes/memorial observado | Caderno aberto e carvão na mão | `hub.elder.stage1.loss`: “Trouxeram a notícia de que a estrada está segura. Pedi os nomes. Disseram que isso atrasaria a próxima escolta.” |
| Ato I | Acolhimento observado | Cobertores junto ao arquivo | `hub.elder.stage1.shelter`: “Recebemos famílias inteiras. Hoje vou escrever seus nomes antes que alguém comece a chamar todos de problema.” |
| Ato II | Escassez após salvar mercadores | Divide pão enquanto examina lista | `hub.elder.stage2.scarcity`: “Se distribuirmos igualmente, todos terão pouco. Se escolhermos quem merece, já teremos perdido mais do que comida.” |
| Ato II | Suprimentos salvos | Lugar vazio na mesa do conselho | `hub.elder.stage2.supplies`: “A mesa tem pão. Deixei uma cadeira para cada homem que não chegou com ele.” |
| Ato III | Vítimas libertadas e descarga observada | Mapa corrigido, mãos trêmulas | `hub.elder.stage3.release`: “Voltaram aos seus filhos. Outros vieram fugindo das cavernas. Não sei onde termina a lista dos salvos.” |
| Ato III | Rede selada observada | Lista de cativos e proteção nos registros | `hub.elder.stage3.seal`: “Estão vivos o bastante para sofrer e ausentes o bastante para serem esquecidos. Não vou encerrar esse livro.” |
| Pré-final | Predomínio judgment + eventos observados | Selo da guarda sobre antigos papéis | `hub.elder.stage4.judgment`: “Eu discutia cada ordem. Hoje só pergunto se os portões vão resistir. É assim que uma cidade ensina um homem a concordar.” |
| Pré-final | Predomínio mercy + eventos observados | Nomes cobertos por símbolos de gratidão | `hub.elder.stage4.mercy`: “Aceitamos cada sacrifício como se fosse o último. Talvez a próxima promessa seja a primeira que eu já não consiga recusar.” |
| Qualquer empate | Stage correspondente | Elementos de cuidado e censura coexistem | `hub.elder.stageN.mixed`: “Tenho o nome de quem ganhou abrigo e o de quem o perdeu. Não vou deixar um apagar o outro.” |

> **Irian — `hub.elder.revealed.acceptance`, stage4, revelação observada e predomínio judgment**  
> “Li o que Vael fez. Não apaguei uma linha. Ainda assim, há uma parte de mim que quer alguém capaz de mandar o mundo parar de ruir. Tenho medo de já estar chamando isso de esperança.”

Essa aceitação é desespero ambivalente, não adesão idêntica à de Bram. Irian fornece as provas de linhagem do módulo 07 mesmo quando sua rotina muda. A fala “Ele reconhece sua marca. Isso não lhe dá o direito de dizer quem você é” permanece acessível como conhecimento registrado; mudanças posteriores não apagam essa prova nem bloqueiam a campanha.

## 7. Seleção de dados — GameState e NPC Core

### 7.1 Estado observável

Estado canônico vem de QuestService/SaveRepository offline ou API online. GameState projeta o resultado depois do commit. Não selecionar diálogo diretamente por um valor de corrupção enviado pelo cliente.

```json
{
  "schema_version": 1,
  "hub_stage": 3,
  "ritual_charge": 9,
  "affinity": {"mercy": 5, "judgment": 4},
  "visible_flags": {
    "act1.desperate_alchemist.deliver_venom": true,
    "hub.serum_effect_observed": true,
    "act3.last_hunger.break_bond": true,
    "hub.abyss_release_observed": true
  },
  "knowledge": {"vael_lineage_revealed": false},
  "npc_states": {"npc.alchemist": "alive"},
  "state_version": 27
}
```

Afinidades do exemplo somam nove decisões; conhecimento ainda não revelado impede menção nominal ao culto de Vael. `visible_flags` é projeção de eventos observados, não dump de efeitos futuros. O exemplo não é save autoritativo aceito por upload.

### 7.2 Regra de catálogo

```json
{
  "rule_id": "lyra_stage3_serum",
  "npc_id": "npc.alchemist",
  "dialogue_id": "hub.lyra.stage3.serum",
  "stage": 3,
  "priority": 100,
  "requires_all": [
    "act1.desperate_alchemist.deliver_venom",
    "hub.serum_effect_observed"
  ],
  "portrait_id": "portrait.lyra.exhausted",
  "service_profile": "alchemist_standard"
}
```

Regras recebem IDs únicos, stage exato, prioridades declaradas e requisitos allowlisted. Para epílogos, resolver usa ending_id antes de stage. Facção/afinidade podem ser filtros adicionais validados. Empate de prioridade entre regras elegíveis é erro de catálogo detectado antes do jogo, não depende de ordem de Dictionary. Todo NPC tem fallback por stage e fallback inicial; nenhuma referência de diálogo/portrait pode faltar no build.

### 7.3 Godot — consulta e resolução

Contrato futuro de GameState: `get_hub_dialogue_context()` retorna cópia do contexto visível; `get_npc_state(npc_id)` já consta no módulo 06. Não adicionar lógica de prioridade dentro de cada NPC.

```gdscript
# Fragmento de npc_core.gd; serviços são referências tipadas da composição.
func interact() -> void:
    var npc_state := GameState.get_npc_state(npc_id)
    if is_dead or npc_state == &"dead" or npc_state == &"hostile":
        return
    var context: Dictionary = GameState.get_hub_dialogue_context()
    var dialogue_id: StringName = dialogue_resolver.resolve_hub(npc_id, context)
    if dialogue_id != &"":
        EventBus.dialogue_requested.emit(npc_id, dialogue_id)
```

Trecho é contrato de implementação, não método já instalado nos anexos. Contexto precisa estar carregado; falha de carregamento mostra feedback e permite retry, sem usar stage0 como se fosse estado confirmado.

```gdscript
# Predicado do resolver para regras já validadas ao carregar o catálogo.
func matches_rule(rule: Dictionary, context: Dictionary) -> bool:
    if int(rule["stage"]) != int(context["hub_stage"]):
        return false
    var flags: Dictionary = context["visible_flags"]
    for flag_id in rule["requires_all"]:
        if not flags.has(flag_id) or flags[flag_id] != true:
            return false
    return true
```

Predicado é intencionalmente pequeno: um validador anterior exige tipos corretos, stages 0–4, IDs existentes e requisitos permitidos. O resolver completo filtra NPC, epílogo/knowledge/afinidade, seleciona prioridade sem empate e retorna fallback válido. Não usar `is_available` que sempre retorna true, nem ler campos desconhecidos como permissões.

Quest/retorno ao hub emite evento confirmado → GameState atualiza projeção → AppearanceController muda cenário/retratos → próxima interação consulta resolver. Não trocar texto no meio de uma fala: sessão de diálogo captura versão; atualização aguarda fim ou reabre com indicação explícita. Serviço de compra valida versão atual independentemente do retrato capturado.

## 8. Gatilhos e precedência

| Prioridade lógica | Informação | Resultado |
|---:|---|---|
| 1 | Campanha encerrada / ending_id | Epílogo do módulo 07, sem diálogo pré-final contraditório |
| 2 | Conhecimento de Vael + condição concreta | Variante nominal de culto/desespero, se elegível |
| 3 | Flag observada de quest | Fala sobre soro, caravana, vítimas ou execução correspondente |
| 4 | Estágio e afinidade observada | Rotina genérica do ato, sem inventar fatos específicos |
| 5 | Empate ou ausência de gatilho específico | Variante mista/fallback por stage |

Não usar prioridade para apagar eventos: o diário mantém fatos anteriores, e a arte compõe objetos compatíveis. Lyra com soro e predomínio judgment continua na rota de dependência; Bram cruel não muda para orgulho artesanal porque uma escolha posterior foi mercy. Falas genéricas mantêm acesso a serviços.

## 9. Critérios de validação futura

- Estágio deriva de marcos, não de relógio; carga/afinidades são aplicadas uma vez.
- Bram, Lyra e Irian têm cobertura de chegada, quatro estágios e empate/fallback.
- Dependência de Lyra exige ingrediente + efeito observado; sem isso não aparece frasco secreto.
- Culto nominal a Vael exige knowledge revelado; pistas anteriores não entregam o clímax.
- NPCs essenciais permanecem alive, sem dano no hub; lojas essenciais continuam atendendo.
- Regra de catálogo não tem empate, referência quebrada ou campo desconhecido aceito.
- Reentrar, carregar save ou falar repetidamente não muda corrupção, ouro ou recompensas.
- IDs estáveis sobrevivem à troca de nomes; aliases de linhas legadas são migrados explicitamente quando houver save externo.
- Testar combinações concretas de soro/caravana/Abismo + afinidade, não apenas uma campanha de cada final.

Esta entrega documenta diálogos e contratos. Não executa Godot, implementa dependência química como sistema de gameplay ou produz sprites/cenas. Os scripts da próxima etapa devem seguir os gates e invariantes acima.
