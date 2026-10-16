# Atlas de pré-produção 3D — campanha completa

**Estado:** proposta de produção, sem níveis ou assets Unreal criados. Os nomes de zonas, quests, inimigos e confrontos vêm do [GDD 04](../GDD/04_mapas_e_quests.md). Percursos, interiores, movimento ambiental e encenação abaixo são **NOVO/EXPERIMENTAL** até revisão editorial e blockout. Esta ficha amplia a [primeira rota](first-route-3d.md), que continua mais detalhada para Cidadela/Bosque. Não usar nomes de locais propostos como IDs de save.

## Regras comuns aos doze mapas

- Campanha autoral, com entrada reconhecível, objetivo visível, retorno seguro e atalhos que abrem de dentro. Os hubs Cidadela, Acampamento das Dunas, Refúgio Subterrâneo e Posto do Limiar permanecem seguros. Nenhuma proposta abaixo cria checkpoint novo no Bosque ou portal procedural obrigatório.
- Escala em metros, duração de travessia, posição de spawn, quantidade de salas e densidade de encontros são **pendentes de blockout**. Antes disso, um número seria falsa precisão. Medir caminho principal e desvio, câmera e leitura de ataques com o tamanho real da cápsula dos cinco heróis.
- Ambiente se move: vento, névoa, água, raízes, máquinas, tecido e luz conforme o bioma. A geometria que bloqueia o herói só muda se houver aviso, alternativa transitável e teste de ida/volta. Partículas não podem esconder ataque, saída, loot ou borda perigosa. Modo de efeitos reduzidos mantém pistas por forma e som.
- Cada interior indicado é uma **proposta de local acessível**, com porta de volta para a mesma âncora, função narrativa ou econômica e política de recompensa a catalogar. Cenário de fachada permanece decorativo até essa ficha existir. Nenhum baú ganha item ou `reward_id` por inferência; somente os dois recipientes da primeira rota têm IDs e conteúdo definidos.
- A escolha principal e suas duas opções são exatamente as do GDD 04. Cinemática, iluminação, inimigo e NPC reagem a fatos persistidos; não concedem progresso. Boss de ato apenas nos mapas 03, 06, 09 e 12. Os demais confrontos são subchefes.

## Ato I — raízes e abrigo

### 01 · `forest_01` — Bosque das Névoas

**Percurso:** portão leste da Cidadela → trilha com primeiro Lobo → bifurcação de pistas externas → aldeia e casa/porão opcionais → arena de Garras → caminho de retorno. A ordem entre decisão da aldeia e arena não é fixa. **Interiores:** casa abandonada e porão já propostos na [ficha da rota](first-route-3d.md); oficina de Bram fica na Cidadela. **Encontros:** Lobo Etéreo, Cultista Cego-Guiado, Espectro de Folhas; Garras de Musgo é subchefe. **Ambiente:** névoa baixa e folhagem oscilam; poço, cerca e borda têm colisão inequívoca. A luz do poço é pista, nunca obstáculo móvel. **Consequência visível:** no retorno, água isolada ou memorial/lista de mortos conforme `act1.spy_hunt`. **Lacuna:** Espectro de Folhas, habitações adicionais e arte 3D ainda não têm ficha de produção.

### 02 · `swamp_02` — Pântano dos Lamentantes

**Percurso proposto:** chegada em margem firme → trilhas elevadas e ilha de observação do soro → laboratório de campo → bacia da Matriarca → retorno por passarela aberta. **Interior proposto:** laboratório pequeno com cadernos de melhora e mutação, acessível antes da decisão `act1.desperate_alchemist`; sem loot obrigatório. **Encontros:** Larvas Parasitas, Afogados de Éter, Harpias de Lama; Matriarca das Águas é subchefe. **Ambiente:** água rasa e juncos ondulam; bolsões tóxicos têm borda e aviso constantes. Passarelas nunca desaparecem sob o jogador. **Consequência:** o laboratório fica vazio/destruído ou com lotes lacrados; Lyra reage no hub. **Lacuna:** profundidade de água, veneno, antídoto e quantidade de rotas dependem de teste de câmera/combate.

### 03 · `temple_03` — Portal do Julgamento

**Percurso proposto:** escadaria externa → pátio de defensores → galeria de selos e raízes ligadas a casas → arena de Silvano → santuário do fragmento → saída aberta. **Interior proposto:** arquivo de selos, com evidência obrigatória da ligação das raízes; leitura disponível mesmo sem coletável. **Encontros:** Defensores Rúnicos, Cultistas Fanáticos, Cães de Guerra; Silvano é boss do ato I. **Ambiente:** raízes pulsam e cristais respondem ao fragmento; crescimento posterior à escolha é visual até navegação e retorno serem testados. **Consequência:** vegetação floresce ou seca conforme `act1.guardian_roots`, sem apagar passagens. **Lacuna:** fases/telegraphs de Silvano e gramática visual do fragmento.

## Ato II — sal e dívida

### 04 · `dunes_04` — Dunas de Sal

**Percurso proposto:** Acampamento das Dunas → marco de ossada → caravana rompida com escolha → crista abrigada da tempestade → arena do Guerreiro de Vidro → retorno ao acampamento. **Interior proposto:** vagão de carga acessível antes da decisão, com manifesto de suprimentos e pessoas; nenhum prêmio condicionado à opção moral. **Encontros:** Escorpiões de Cristal, Nômades Corrompidos, Gárgulas de Areia; Guerreiro de Vidro é subchefe. **Ambiente:** areia e tecido se movem; rajadas podem mudar visibilidade, mas rota principal, borda e telegraphs permanecem legíveis. **Consequência:** memorial ou fila de ração no acampamento/Cidadela segundo `act2.lost_caravan`. **Lacuna:** desenhar os nômades como pessoas afetadas, sem equivaler sua cultura à corrupção.

### 05 · `valley_05` — Vale dos Sacrifícios

**Percurso proposto:** ravina de entrada → pontes com mirantes de retorno → prisão/registro do líder cultista → altar da escolha → arena do Executor de Bronze → saída para a Cripta. **Interior proposto:** câmara de inscrições que expõe a relação entre altar, fontes e tumbas antes de `act2.flame_ritual`. **Encontros:** Ritualistas de Sangue, Harpias de Obsidiana, Hienas de Éter; Executor de Bronze é subchefe. **Ambiente:** bandeirolas e chamas se agitam; pontes mantêm largura e guarda visual, sem quedas surpresa durante a escolha. **Consequência:** o altar registra a via usada; a penalidade conhecida de vitalidade, se escolhida, expira pelo contrato do GDD. **Lacuna:** custo temporário e acessibilidade das travessias para invocações.

### 06 · `crypt_06` — Cripta de Ossos

**Percurso proposto:** porta funerária → galerias de nomes → ossuário com almas visíveis → câmara de Malignus → selo/fragmento → atalho de saída. **Interior proposto:** nicho de registros de familiares, separado da futura dungeon opcional de Namar; informações essenciais não podem ficar na dungeon. **Encontros:** Esqueletos Cristalizados, Múmias Rúnicas, Sombras Devoradoras; Malignus é boss do ato II. **Ambiente:** rios místicos e luz azul se movem; correntes e vozes indicam a pressão do selo antes de `act2.ossuary`. **Consequência:** eco livre ou correntes reforçadas aparece no retorno, sem prender o jogador. **Lacuna:** silhueta e fases de Malignus; recompensa segue a categoria de boss do GDD 03.

## Ato III — água, máquina e fome

### 07 · `sewers_07` — Esgotos de Éter

**Percurso proposto:** Refúgio Subterrâneo → plataforma de canais → diagrama hidráulico e moradias inferiores visíveis → válvula → ninho da Mãe das Proles → retorno por escada protegida. **Interior proposto:** sala de manutenção para ler o mapa de fluxo antes de `act3.purification_valve`. **Encontros:** Ratos de Éter, Mutantes de Esgoto, Aranhas-Luminosas; Mãe das Proles é subchefe. **Ambiente:** escoamento, vapor e válvulas animados; inversão de fluxo não bloqueia saída nem cria dano no refúgio. **Consequência:** resíduos de superfície ou marcas de inundação inferior são mostrados depois da confirmação. **Lacuna:** alturas de canal, limites de natação e resposta dos NPCs do refúgio.

### 08 · `foundry_08` — Fundição Proibida

**Percurso proposto:** porta de manutenção → esteiras de produção → galeria onde sobreviventes são audíveis → núcleo de energia → arena do Autômato Forjador → saída de serviço. **Interior proposto:** posto de controle com leitura do estado de contenção antes de `act3.energy_core`. **Encontros:** Constructos de Bronze, Autômatos, Elementalistas de Magma; Autômato Forjador é subchefe. **Ambiente:** engrenagens, pistões e fundição em movimento; peças com colisão só operam dentro de ciclos avisados, com rota estável de retirada. **Consequência:** armas reforçadas ou contenção selada aparece em cenário/NPC, sem apagar a escolha. **Lacuna:** separar autômato comum do subchefe por silhueta e padrão, e testar NavMesh nas esteiras.

### 09 · `abyss_09` — Abismo da Voracidade

**Percurso proposto:** abertura na alvenaria → corredor de vínculos com vítimas reconhecíveis → antecâmara de retirada → arena de Gorgoroth → foco/fragmento → saída limpa. **Interior proposto:** abrigo escavado por vítimas, com depoimento obrigatório sobre vínculo e descarga antes de `act3.last_hunger`. **Encontros:** Abominações de Carne, Vermes de Éter, Parasitas Espirituais; Gorgoroth é boss do ato III. **Ambiente:** paredes respiram e Éter pulsa; colisão do piso e posição do foco permanecem estáveis durante o combate. **Consequência:** vínculo rompido ou selado altera som/luz, sem repetir escolha em reload. **Lacuna:** apresentação de horror com opção de reduzir efeitos e fases do boss.

## Ato IV — limiar e autoridade

### 10 · `gardens_10` — Jardins Suspensos de Éter

**Percurso proposto:** Posto do Limiar → terraços de jardins → pedestal que mostra a origem da barreira → arena do Arconte de Prata → passagem superior. **Interior proposto:** observatório de registros da barreira, acessível antes de `act4.judgment_throne`. **Encontros:** Anjos Caídos, Sentinelas de Prata, Espectros Celestiais; Arconte de Prata é subchefe. **Ambiente:** jardins e fissuras estelares se movem; terraços têm parapeitos legíveis e ponto de retorno. **Consequência:** almas liberadas ou guardas espectrais aparecem ao longe; nenhum aliado necessário some do hub. **Lacuna:** distinguir anjos de entidades religiosas reais e traduzir o custo conhecido do pedestal em UI.

### 11 · `bridge_11` — Ponte dos Lamentos

**Percurso proposto:** cabeceira segura → trechos com coberturas e visão do selo → estação de decisão → arena do Guardião dos Portões → soleira de Vael. **Interior proposto:** torre lateral de vigia, ligada à cabeceira, que mostra rachaduras e socorro dos aliados antes de `act4.last_gate`. **Encontros:** Guardas de Vael, Constructos Rúnicos Supremos, Manifestações da Bruma; Guardião dos Portões é subchefe. **Ambiente:** tempestade e relâmpagos sinalizados; ponte nunca quebra em área transitável sem nova malha de navegação e telegraph. **Consequência:** socorro interrompido ou canalização do herói tem leitura clara, inclusive duração da penalidade temporária. **Lacuna:** espaço de esquiva suficiente para os cinco heróis e invocações.

### 12 · `heart_12` — Coração de Éter

**Percurso proposto:** soleira → registros finais legíveis → plataforma de cristal negro → arena de Vael → decisão `act4.heart_fate` → epílogo. **Interior proposto:** nenhum desvio de saque ou combate comum; o arquivo de revelação no Posto do Limiar fornece a genealogia antes da arena, conforme GDD 07. **Encontro:** Vael, boss final; manifestações pertencem à luta, não ao bestiário comum. **Ambiente:** eclipse e fissuras reagem às fases sem mover a borda da arena nem esconder hitboxes. **Consequência:** Ordem ou Ascensão produz epílogos diferentes, modulados pelas escolhas anteriores; a luta é uma vitória real. **Lacuna:** fases, câmera, música, finalização e testes de todas as flags relevantes.

## Inventário de produção antes de criar a parte jogável

1. Fechar, para **cada** mapa, planta de rotas/retorno e largura de combate, lista de portas, geometria estática/móvel, encontros, arena, assets e dependências de quest. As propostas acima ainda não são plantas métricas.
2. Fechar [elenco e bestiário](campaign-cast-and-bestiary-3d.md): cinco protagonistas, cinco NPCs do conselho, 33 arquétipos comuns, oito subchefes e quatro bosses; decidir o que compartilha rig/material sem perder silhueta.
3. Catalogar itens, vendedores, tesouros e recompensas únicos de todos os mapas. Nada além dos dois baús da primeira rota recebe conteúdo automaticamente.
4. Revisar editorialmente locais/interiores propostos e registrar IDs definitivos; depois produzir conceito, blockout visual e testes de navegação **sem loop jogável**. Somente após cobertura da campanha e aprovação de um gate de conteúdo iniciar gameplay Unreal.
