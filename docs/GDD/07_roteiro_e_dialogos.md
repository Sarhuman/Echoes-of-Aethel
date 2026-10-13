# Módulo 07 — Roteiro, diálogos e confronto final

**Projeto:** Echoes of Aethel · **Versão:** 1.0 · **Estado:** roteiro de produção; cenas, áudio e scripts pendentes.

> **Spoilers completos.** A Sprint 6 de arquitetura já ocupa o módulo 06. Esta entrega narrativa usa o caminho solicitado `07_roteiro_e_dialogos.md`, sem substituir a arquitetura Godot.

Referências: [classes](01_classes_e_atributos.md), [mapas e escolhas](04_mapas_e_quests.md), [combate/HUD](05_combate_e_hud.md) e [serviços de diálogo](06_arquitetura_software.md). Este módulo rege encenação e falas; não muda opções, recompensas ou finais dos módulos anteriores.

## 1. Convenções de apresentação

Caixas azuis #17213B→#29334E, borda dupla marfim/prata, retratos 64×64 e texto claro ampliável. Citações são o fallback legível em Markdown; a engine aplica borda/fundo. Sem ASCII decorativo ou dependência de HTML para ler a história.

Cada fala tem speaker, line_id, variante, condição e texto pt-BR. Usar nome do personagem escolhido por placeholder validado, sem inserir conteúdo como código/markup executável. Retratos, som e câmera são apresentação; não executam escolhas ou alteram HP.

Cenas podem ser puladas e relidas no diário. Pular chega ao mesmo estado narrativo final e registra os mesmos objetivos uma vez, sem pular combate/decisão. Não disparar recompensas ao terminar uma animação. Online, cutscene só pausa o encontro mediante estado autoritativo explícito; offline pausa simulação e timers juntos.

## 2. Prólogo — A cidade que ainda respira

### Cena P01 — O último sino

**ID:** `prologue.last_bell` · **Entrada:** nova campanha · **Local:** muralha da Cidadela.

Tela escura. Três batidas de sino, a última quebrada. Surge um cristal frio sobre telhados apertados. A câmera desce por roupas penduradas, uma carroça vazia e mãos marcadas de poeira. Música original de timbres 16-bit: baixo contido e notas espaçadas, sem melodia triunfal.

> **Narrador — P01.01**  
> “Aethel aprendeu a medir o tempo pela luz do Éter. Quando o Coração se partiu, os relógios continuaram. Foi o mundo que perdeu o ritmo.”

> **Narrador — P01.02**  
> “A Bruma ergueu os mortos, mudou as feras e ensinou os vivos a temer a água. Atrás destes portões, um fragmento ainda respira.”

O herói chega. Maera segura a grade enquanto examina uma marca luminosa na mão dele. Não mostra símbolo exclusivo de Vael nesta etapa.

> **Maera — P01.03**  
> “Guarde essa mão. Aqui dentro, qualquer brilho vira promessa.”

| Classe | Resposta P01.04 |
|---|---|
| Tecelão | “Não vim prometer. Vim fechar o que minha ordem abriu.” |
| Sentinela | “Se encontrarem uma cura, deixo o brilho com vocês.” |
| Bastião | “Conheço estes portões. Foi o lado de fora que mudou.” |
| Invocadora | “Os que vieram comigo não precisam de abrigo. Eu preciso.” |
| Inquisidor | “Já segui luzes que mentiam. Esta também terá de responder.” |

> **Maera — P01.05**  
> “Então entre. O conselho está junto ao cristal. Se oferecerem respostas fáceis, olhe para as mãos de quem fala.”

**Saída:** controle liberado em zona segura; objetivo visitar o conselho. Tutorial de movimento/interação não permite dano a NPCs. Conversas ambientais não são dilemas morais.

### Cena P02 — O conselho dos que ficaram

**ID:** `prologue.council` · **Entrada:** primeira interação no salão do cristal.

Conselho funcional: Maera responde pela guarda; Irian pelo arquivo/fragmento; Lysa pelo cuidado e água; Oren pelas oficinas; Sera representa abastecimento e mercadores. Sem um monarca infalível: o grupo discorda e assume limites.

> **Irian — P02.01**  
> “Cinco Santuários mantinham o Éter no seu leito. Vael tentou abrir o Coração durante o eclipse. Agora temos um fragmento e quatro ausências.”

> **Lysa — P02.02**  
> “O cristal afasta a Bruma dos portões. Não a tira do que já trouxeram para dentro. Preciso de água limpa, não de outra promessa.”

> **Sera — P02.03**  
> “A última caravana chegou sem condutor. Havia comida. Ninguém teve coragem de comer antes que ela examinasse.”

> **Oren — P02.04**  
> “Posso reforçar escudos. Não posso reforçar uma estrada inteira.”

> **Maera — P02.05**  
> “Há gente desaparecendo no Bosque. Se mandarmos toda a guarda, deixamos estes portões sem ninguém. Se não mandarmos ninguém, a cidade acaba por dentro.”

O cristal pulsa ao herói se aproximar. Irian recolhe a mão antes do contato.

> **Irian — P02.06**  
> “Há pedras que lembram de quem nunca vimos. Não toque. Ainda.”

> **Irian — P02.07**  
> “Silvano guarda uma parte do Coração. Além dele, as ruínas e o Abismo escondem outras duas. A última luz responde ao Pináculo. Não lhe pedimos que salve tudo hoje. Comece pela estrada.”

> **Herói — P02.08**  
> “E se aquilo que eu trouxer não for uma cura?”

> **Lysa — P02.09**  
> “Traga o que viu. Nós decidiremos o que fazer com isso. Não carregue nossa ignorância como se fosse culpa sua.”

**Saída:** objetivo Bosque disponível, checkpoint Cidadela registrado. Conselho não sabe da linhagem e não antecipa o diagnóstico final do Éter. Equipamento inicial é concedido pelo caso de uso de criação, não outra vez nesta cena.

### Cena P03 — A soleira

> **Oren — P03.01**  
> “Aço não promete. Só precisa aguentar mais um golpe.”

> **Lysa — P03.02**  
> “Vida, Éter. Dois frascos, dois limites. Se estiver inteiro, guarde o remédio para depois.”

> **Irian — P03.03**  
> “A névoa não é natural. Ela respira com o sangue de Silvano. Não confunda silêncio com caminho livre.”

> **Maera — P03.04**  
> “Traga nomes. Estamos cansados de contar sombras.”

Saída para mapa 01 depende de criação/tutorial concluídos; nenhum prazo real faz a cidade adoecer enquanto o jogador aprende controles.

## 3. Roteiro de passagem pelos atos

As quests e suas 24 opções continuam no módulo 04. Falas abaixo ligam os marcos principais e pistas; não acrescentam escolhas de recompensa ou gates secretos.

| Marco / ID | Encenação | Fala e informação liberada |
|---|---|---|
| Silvano antes / `act1.silvano.intro` | Cajado-espada hesita diante da mão marcada | “Essa luz em sua mão… não. Ele não pisaria aqui outra vez.” |
| Silvano depois / `act1.silvano.defeat` | Raízes desenham brasão incompleto e recuam | “A marca em sua alma… você carrega a autoridade Dele.” |
| I→II / `transition.act2` | Carroça deixa árvores mortas; vento leva sal | Narrador: “Além das árvores, o vento não trazia folhas. Trazia sal — e nomes raspados da pedra.” |
| Malignus antes / `act2.malignus.intro` | Ossos se inclinam como uma corte | “Não entre como um ladrão. Seu sangue conhece esta casa.” |
| Malignus depois / `act2.malignus.defeat` | A máscara cai sem revelar olhos | “Cada alma que libertou pavimentou um trono. Não pergunte de quem. Pergunte por que ele cabe em você.” |
| II→III / `transition.act3` | Água escorre das tumbas para tubos | Narrador: “Sob as tumbas, algo ainda trabalhava. Nenhum dos mortos lembrava para quem.” |
| Gorgoroth antes / `act3.gorgoroth.intro` | Bocas repetem vozes vistas no hub, sem imitar uma pessoa que não existe no save | “Você trouxe fome com nome de cuidado.” |
| Gorgoroth depois / `act3.gorgoroth.defeat` | Voz interna, sem caixa de NPC visível | “Nós fomos feitos para alimentar um recipiente. Não para escolher quem ele seria.” |
| III→IV / `transition.act4` | Fragmentos pulsam no limiar; Cidadela permanece protegida | Narrador: “A escada subia onde já não havia céu. O cristal em sua bolsa pulsava antes de cada passo.” |

No arquivo de Irian, três registros independentes confirmam linhagens marcadas antes do eclipse: assinatura rúnica da ordem, registro de sangue da guilda e nascimento sob custódia da seita. As variantes de classe trazem evidência equivalente da tribo/guarda. Registros vistos nos atos I–III mostram só fragmentos; no limiar final, comparação dos selos libera a conclusão. Informação essencial não depende de coletável aleatório ou único diálogo perdível.

> **Irian — `liminal.archive.revelation`**  
> “Não encontrei uma viagem. Encontrei uma genealogia. A mesma marca, atravessando gerações. Não foi o eclipse que criou sua família. Foi ele que acordou o que Vael havia deixado nela.”

O herói pode ler essa prova antes da arena. A confirmação dramática acontece com Vael; o pai não é a única fonte da verdade.

## 4. Diálogos dinâmicos e deterioração do hub

Estágios e flags seguem módulo 04. Aplicar variantes por **fato observado**, não somente pelo contador numérico. Ordem de seleção: epílogo final → flag concreta visível → estágio/afinidade → fallback inicial. Em empate, variante mista. Os eventos ocultos continuam secretos até seu trigger; cruzar um ato não revela automaticamente uma consequência ainda pendente.

### 4.1 Oren — Ferreiro

| Estágio | Visual | Linha de misericórdia / acolhimento | Linha de julgamento / contenção | Variante mista |
|---|---|---|---|---|
| Chegada | Avental gasto, luz âmbar, mãos firmes | “Aço não promete. Só precisa aguentar mais um golpe.” | Mesma fala inicial | Mesma fala inicial |
| Ato I | Olheiras; plantas secas ou pólen na janela | “O aprendiz está febril. Disse que nunca dormiu tão bem. Eu fico na oficina.” | “Trouxeram armas sem dono. Não vou vendê-las como se nunca tivessem mãos.” | “Consertei a fechadura e o portão. Ainda não sei de qual lado ficaram os feridos.” |
| Ato II | Metal reaproveitado; placas de sentença ou fila de reparos | “Há gente viva na fila. Isso devia bastar. Mas nenhum deles tem mais casa.” | “Pedem lâminas com o nome da lei. Ninguém me traz o nome de quem vai recebê-las.” | “Uma peça vai para a guarda, outra para uma carroça. As duas voltam quebradas.” |
| Ato III | Tubos e máquina nova; fuligem no retrato | “A máquina trabalha à noite. Às vezes pede água com a voz de alguém.” | “Querem que eu fabrique mais. Perguntei quem vai segurar tudo isso. Não responderam.” | “O metal chega. Os homens que o traziam, não.” |
| Pré-final | Oficina organizada demais; estandarte preso com prego torto | “Estão chamando sua dor de bênção. Não deixe que cobrem a mesma dos outros.” | “O escudo protege quem está atrás dele. Começaram a usá-lo para empurrar.” | “Ainda sei fazer uma arma. Esqueci como explicar por que não quero fazer outra.” |

Cada linha tem ID `hub.oren.stage_N.mercy|judgment|mixed`. A flag `shelter_villagers` só cita o aprendiz febril após contaminação observada; sem essa flag, substituir por “As famílias que chegaram dormem na oficina. Eu trabalho devagar para não acordá-las.” Oren não morre nem fecha preparação essencial.

### 4.2 Lysa — Alquimista

| Estágio | Visual | Linha de misericórdia | Linha de julgamento | Variante mista |
|---|---|---|---|---|
| Chegada | Vidros limpos, retrato atento | “Se a água mudar de cheiro, venha antes da sede.” | Mesma fala inicial | Mesma fala inicial |
| Ato I | Panos violetas; mesa coberta, sem risco jogável | “Melhoraram. É o que me dizem. Não sei por que evitam os espelhos.” | “Tenho remédios sobrando. Isso nunca foi boa notícia.” | “Uma cura chegou junto de outra febre. Separei os frascos. Não consegui separar as famílias.” |
| Ato II | Cisternas isoladas e cadernos rasurados | “Eles sonham com quem perderam. Depois deixam de lembrar o rosto.” | “Disseram que não devo ouvir as vozes do ossuário. Como se eu escolhesse o que escuto.” | “Hoje tratei a insônia de um guarda e o silêncio de uma mãe. Não escrevi receita para nenhum dos dois.” |
| Ato III | Máscara repousa ao lado dos frascos; janelas vedadas | “Trouxemos gente das profundezas. A superfície não soube recebê-los inteira.” | “Contivemos a descarga. Continuo ouvindo os que ficaram no lugar dela.” | “O mapa mudou os canos. A água continua chegando a alguma boca.” |
| Pré-final | Símbolos sagrados sobre antigos rótulos | “Querem beber o que tocar sua mão. Eu ainda prefiro ferver a água.” | “A cura virou ordem. Já não perguntam se alguém quer recebê-la.” | “Se eu não escrever seu nome neste frasco, talvez ainda se lembrem de que é só remédio.” |

IDs `hub.lysa.stage_N.variant`. Sonhos exigem `free_captives` observado; vozes do ossuário exigem `reinforce_seal`. A fala sobre espelhos exige `deliver_venom`; caso contrário: “O tratamento parou. A fila, não.” Atendimento e compra de poções continuam conforme módulo 05, sem preço punitivo escondido por afinidade.

### 4.3 Sera — Mercadora / abastecimento

| Estágio | Visual | Linha de misericórdia | Linha de julgamento | Variante mista |
|---|---|---|---|---|
| Chegada | Balança, caixas pequenas e capa ocre | “Posso contar sacos. Pessoas não cabem tão bem nas contas.” | Mesma fala inicial | Mesma fala inicial |
| Ato I | Cobertores ou objetos de moradores ausentes | “Abrimos espaço para os que vieram. Ninguém perguntou ao estoque.” | “Estas botas têm nome por dentro. Vou guardar até alguém vir buscá-las.” | “A cidade cresce em camas e diminui em cadeiras.” |
| Ato II | Barracas vazias ou armazém cheio com memorial | “Os mercadores voltaram vivos. Não consegui vender essa notícia como pão.” | “Temos farinha. Os homens que a trouxeram não vão comer.” | “Ganhei uma estrada e perdi uma lista de clientes.” |
| Ato III | Novas peças; bilhetes de desaparecidos | “Eles voltaram para as famílias. Ainda perguntam por lugares que já não existem.” | “Tudo chega com escolta. Até o silêncio.” | “Os preços continuam na placa. Ninguém pergunta mais quanto custa o resto.” |
| Pré-final | Estandarte do herói ou selo da guarda | “Querem uma lembrança sua antes de partir. Eu devia vender corda, não relíquias.” | “O inventário está certo. Foi a cidade que deixou de conferir os nomes.” | “Deixei um lugar vazio na feira. Não sei para quem estou guardando.” |

IDs `hub.sera.stage_N.variant`. Falas do Ato II dependem diretamente da caravana, não da maioria moral do ato. Sera permanece; outros mercadores ausentes recebem substitutos operacionais. Barraca fechada decorativa não bloqueia o único acesso a inventário/loja.

### 4.4 Vozes do conselho

> **Maera — estágio misto III**  
> “Os portões ainda obedecem. As pessoas já não confiam em quem segura a chave.”

> **Irian — pré-final**  
> “Ele reconhece sua marca. Isso não lhe dá o direito de dizer quem você é.”

No hub não há combate, dano de doença, flash hostil ou remoção dos quatro NPCs essenciais. Mudanças são arte, relatos e conflitos sociais. A câmera não encena massacres contradizendo a zona segura.

## 5. Mapa 12 — Entrada no Coração de Éter

### Cena F01 — O anfitrião

**ID:** `final.vael.arrival` · **Pré-condições:** mapas 10/11 resolvidos, três fragmentos dos guardiões registrados, acesso à arena autorizado. O fragmento da Cidadela continua no hub; Vael carrega o quinto.

Plataforma negra sobre nuvens interrompidas. Ao fundo, Aethel aparece como camadas de pixel art: bosque, ruínas, canais e muralhas. O eclipse parece imóvel. Vael está de costas, mãos vazias, arma nenhuma empunhada. Vira-se lentamente e abre os braços antes que a música de combate comece.

> **Vael — F01.01**  
> “Você demorou menos do que eu temia.”

> **Herói — F01.02**  
> “Não vim para ser recebido.”

> **Vael — F01.03**  
> “Mas foi recebido em todos os lugares. Por uma porta que precisava ser fechada. Por alguém que precisava ser salvo. Eu cuidei para que nunca lhe faltasse uma razão.”

A câmera mostra a mão marcada, depois os fragmentos. Vael não toma controle do personagem por um tempo indefinido; botão de avanço e skip permanecem disponíveis.

### Cena F02 — O recipiente

**ID:** `final.vael.revelation` · Monólogo fixo seguido de duas referências às escolhas confirmadas.

> **Vael — F02.01**  
> “Quando perfurei o Coração, pensei que pudesse suportar sozinho o que havia nele. O eclipse me devolveu uma forma que já não podia reunir os fragmentos. Eu precisava de alguém que o Éter reconhecesse sem rejeitar.”

> **Vael — F02.02**  
> “Minha marca atravessou linhagens muito antes daquela noite. Sua família, a ordem que o ensinou, as mãos que o esconderam… não são acaso. Você é meu sangue. Meu filho, minha filha, meu herdeiro. O nome muda menos do que imagina.”

> **Vael — F02.03**  
> “Não bastava nascer com a marca. Eu precisava que a carregasse até onde nenhum coração chega inteiro. Preparei os selos, as promessas de cura, as portas que só cedem quando alguém paga. Dei a cada caminho uma razão justa.”

> **Vael — F02.04**  
> “Cada misericórdia ensinou sua alma a conter outra alma. Cada julgamento ensinou-a a impor um limite. A mesma energia, duas formas de obedecer. Chamaram isso de bondade, de lei, de sacrifício. Eu chamo de preparação.”

> **Vael — F02.05**  
> “Você caminhou onde desenhei seus passos. Tome seu lugar ao meu lado… ou ofereça, por fim, aquilo que ainda insiste em chamar de seu.”

Vael reivindica autoria total, mas o roteiro não confirma que tenha controlado cada pessoa ou tragédia. Registros provam marca e circuitos, não onisciência. O jogador mudou destinos concretos; o vilão tenta apropriar-se dessas escolhas.

### 5.1 Referências variáveis ao percurso

Selecionar uma fala de ato I e uma de ato III na tabela. Nunca citar os dois resultados incompatíveis da mesma quest. Fallback se estado incompleto é erro de integridade bloqueando início da cena, não inventar escolhas.

| Flag da opção | Fala de Vael |
|---|---|
| `act1.spy_hunt:shelter_villagers` | “Recebeu uma aldeia. A água aprendeu o caminho até a cidade antes de você compreender o que havia trazido.” |
| `act1.spy_hunt:judge_suspects` | “Protegeu a estrada com uma sentença. Os mortos ficaram para me mostrar por onde ela passava.” |
| `act3.last_hunger:break_bond` | “Abriu o vínculo por aqueles que ainda chamavam alguém pelo nome. Foi o alívio deles que afinou o Coração.” |
| `act3.last_hunger:seal_creature` | “Manteve a dor quieta para que o mundo não tremesse. Quieto não significa vazio.” |

> **Herói — F02.06**  
> “Você preparou os caminhos. Não foi você quem carregou os feridos.”

> **Vael — F02.07**  
> “E, ainda assim, trouxe tudo o que eu precisava.”

## 6. Confronto por classe

Usar `class_id` canônico; as falas retomam módulo 01. Todas levam ao mesmo início de combate, sem buff gratuito, perda de item ou escolha extra.

| Classe / ID | Vael | Resposta do herói | Retrato e reação |
|---|---|---|---|
| Tecelão / rune_weaver | “Minha mão desenhou a primeira runa que sua ordem chamou de descoberta.” | “Então aprendi o bastante para apagar sua assinatura.” | Espanto → determinação; cajado desfaz uma runa de cenário |
| Sentinela / shadow_sentinel | “A marca nas suas veias não é uma doença. É minha herança.” | “Herança se recusa. Começo pelo seu nome.” | Fecha e abre a mão marcada; adaga permanece em guarda |
| Bastião / twilight_bastion | “Você fechou os portões; eu fechei os céus. Conhecemos o preço de proteger os nossos.” | “Eu ainda escuto quem ficou do lado de fora. Você só escuta a si mesmo.” | Baixa escudo por um instante e o ergue sem ativar defesa mecânica |
| Invocadora / thorn_summoner | “Seus ancestrais não me esqueceram. Fugiam de mim através de você.” | “E chegaram longe o bastante para me ensinar a dizer não.” | Vozes silenciam, espinho branco surge; não cria invocação |
| Inquisidor / sacred_flame_inquisitor | “Sua chama e a minha nasceram da mesma fome. O fogo não distingue pai e filho.” | “Então terá de aprender a distinguir escolha de obediência.” | Afasta símbolo da seita sem alterar inventário; chama permanece |

IDs: `final.class.<class_id>.vael`, `.hero`, `.reaction`. Variante de tratamento familiar é parametrizada; não substituir origem adulta por nascimento durante o eclipse.

> **Vael — `final.combat.begin`**  
> “Então mostre-me o que ainda não consegui lhe ensinar.”

A aura forma a arma somente agora. Fade musical termina, input retorna e o servidor/adapter cria estado de combate ativo. O primeiro ataque tem antecipação e janela de preparação; nenhum dano acontece durante a cutscene.

## 7. Roteiro da batalha final

Encenação ligada a estados autoritativos, não a número de frames ou duração da música. Padrões abaixo são diretrizes para protótipo, sem HP/dano balanceados. Arena conserva corredor seguro, colisão consistente e nenhuma exigência de habilidade de classe específica.

| Fase | Gate HP | Encenação / fala | Diretriz de combate |
|---|---|---|---|
| I — O Arquimago | 100% até >65% | Vael em forma quase humana: “Ainda procura a regra certa para me vencer.” | Projéteis em linhas e selo circular, antecipações legíveis; recuperação entre padrões |
| II — O Pai do Eclipse | ≤65% até >30% | Corpo fratura; vozes citam uma escolha real: “Não precisei obrigá-lo. Precisei que acreditasse.” | Alternar ondas com abertura e manifestações limitadas; nunca sobrepor zonas sem saída |
| III — Sol-Negro | ≤30% até >0% | Silhueta estelar; voz perde certeza: “Se não tomar meu lugar, o que fará com tudo isto?” | Sequência de padrões anteriores, mantendo telegraph; janela de resposta depois de cada sequência |
| Vitória | HP=0 confirmado | Arma desfaz; eco permanece, corpo sem autoridade ativa | Cancelar projéteis/DoT/invocações hostis; gravar recompensa única e abrir decisão final |

Cada gate dispara uma vez. Se um golpe cruza dois thresholds, enfileirar transições em ordem sem executar dois padrões no mesmo tick; a ficha de combate definirá política de overkill e pausa de transição. Interrupção de cena não reseta vida nem reabre recompensa. Cutscene de fase pode ser resumida no retry; golpe em andamento não continua invisível durante fala.

Escolhas não tornam a luta impossível nem criam um segundo multiplicador de corrupção sobre boss. Referências mudam diálogo/visual; padrões dependentes de flags, se adicionados, exigem mesmo orçamento e teste das duas rotas. Fase adicional de Eclipse continua pendente conforme módulo 05; não habilitada automaticamente por esta tabela.

### 7.1 Morte e tentativa seguinte

Normal: morte volta ao checkpoint seguro, preserva escolhas e flags; intro longa pode ser pulada na próxima entrada. Custo temporário do mapa 11 expira conforme módulo 04 e não é cobrado de novo. Recompensa só ocorre em vitória validada. Eclipse: morte permanente segue módulo 05, sem “ressurreição pela cutscene”.

## 8. Após Vael — Uma escolha que ainda é sua

### Cena F03 — O eco

A plataforma se estabiliza. O fragmento de Vael se solta; os três recuperados respondem. Uma projeção mostra a Cidadela e seu cristal, sem removê-lo antes da decisão. O ritual conecta os fragmentos por ressonância, não por teletransporte instantâneo omitido no roteiro.

> **Vael, eco — F03.01**  
> “Pode devolver-lhes um mundo que reconhecem. Ou dar-lhes um mundo que ainda consiga viver.”

> **Herói — F03.02**  
> “Você fala como se só houvesse essas duas coisas.”

> **Irian, memória — F03.03**  
> “Nem tudo o que resta pode ser salvo. Isso não torna tudo o que se perde uma necessidade.”

Painel de decisão tem **exatamente duas opções**, IDs do módulo 04:

| Opção | Texto visível | Benefício pretendido / pista conhecida |
|---|---|---|
| `restore_order` — O Fim da Ordem | “Restaurar o Coração para preservar a cidade e limitar a Bruma.” | O filtro estabiliza a proteção; registros avisam que regiões externas dependem do Éter extraído |
| `embrace_ascension` — A Ascensão Estelar | “Absorver os fragmentos para reconstruir Aethel e cessar sua agonia.” | O Éter pode recompor matéria; registros mostram dissolução de fronteiras entre almas |

“O Fim da Ordem” é o título dramático da rota de restauração, significando o fim da jornada **sob a Ordem**, não a destruição do filtro; ID/efeito continuam `restore_order`. No resumo público pode permanecer “Final da Ordem” para evitar ambiguidade.

Confirmação explica irreversibilidade na campanha sem revelar epílogo completo. Vitória/loot já foram gravados; fechar o jogo neste painel retoma F03 sem nova recompensa. Vael morto não passa a ser aliado obrigatório nem retorna fisicamente em qualquer final.

## 9. Final A — O Fim da Ordem

**ID:** `ending.order` · **Escolha:** `restore_order`.

O herói insere a própria marca no filtro. Os fragmentos retomam o circuito; o cristal da Cidadela pulsa em resposta. A Bruma recua dos portões. Uma respiração coletiva acompanha o primeiro sino inteiro.

> **Maera — E01.01**  
> “Os portões vão aguentar.”

> **Herói — E01.02**  
> “Por quanto tempo?”

> **Maera — E01.03**  
> “Hoje, só consigo lhe dizer hoje.”

A câmera atravessa a feira, segue além da muralha e revela árvores sem brilho, fontes imóveis e assentamentos sem Éter vital. A cidade recebe salvação material; o circuito perpetua a extração e a estrutura de controle de Vael.

> **Lysa — E01.04**  
> “A água está limpa. Não tenho como mandá-la para todos.”

> **Irian — E01.05**  
> “Escrevi os nomes que trouxeram. Os que não chegaram também pertencem a esta história.”

> **Narrador — E01.06**  
> “A Cidadela voltou a contar os dias. Do lado de fora, o mundo aprendeu a esperar por uma porta que já não precisava se abrir.”

Último quadro: memorial com nomes preservados conforme flags. O herói não recebe absolvição automática e a ordem social varia conforme quem sobreviveu; não substituir tudo por um “final ruim” uniforme.

## 10. Final B — A Ascensão Estelar

**ID:** `ending.ascension` · **Escolha:** `embrace_ascension`.

Os fragmentos entram no herói por ressonância. Sua sombra atravessa a plataforma e vira horizonte. Águas retomam cursos, a floresta floresce, muros rachados se fecham. Não há explosão triunfal: a música repete uma nota que antes pertencia a muitas vozes.

> **Herói — E02.01**  
> “Eu ouvi vocês. Não vou deixá-los no escuro.”

A praça reaparece limpa. Pessoas se levantam ao mesmo tempo. Sera examina a balança, mas esquece por que havia guardado botas com nomes.

> **Sera — E02.02**  
> “Há lugar para todos.”

> **Oren — E02.03**  
> “Há lugar para todos.”

> **Lysa — E02.04**  
> “Há lugar para todos.”

Irian tenta escrever um nome. A mão repete o símbolo do Coração. Retratos têm olhos semelhantes; a tranquilidade cobra autonomia e memória. O projeto de soberania de Vael ascende no sucessor, embora o corpo de Vael esteja derrotado.

> **Narrador — E02.05**  
> “Aethel voltou a respirar. Já não sabia dizer quem respirava.”

Último quadro: o nome do herói se dissolve em luz; o save preserva a identidade e decisões para registro, não as apaga realmente do arquivo. Se o jogador vê narração após transformação, ela não implica controlador livre em campanha encerrada.

## 11. Epílogos condicionais

Aplicar uma entrada por quest/flag relevante, mais final base. Os exemplos abaixo mostram o mesmo fato sob os dois finais; não usar afinidade para inventar sobreviventes ou apagar mortos.

| Decisão confirmada | Ordem | Ascensão |
|---|---|---|
| Acolher aldeia | Famílias permanecem na cidade; nomes dos que adoeceram entram na enfermaria/memorial | Famílias ocupam casas refeitas, mas já não reconhecem entre si o parentesco |
| Executar suspeitos | Memorial externo sem visitantes; parentes guardam objetos dentro do hub | O bosque cresce sobre o memorial; inscrições se tornam o mesmo símbolo |
| Salvar mercadores | Sera mantém uma banca de auxílio sob escassez | Banca cheia, sem nomes individuais no livro de entregas |
| Salvar suprimentos | Armazém abastecido e mesa vazia para os condutores perdidos | A mesa é removida; a cidade não lembra por que havia lugar vazio |
| Libertar vítimas do Abismo | Reencontros registrados; refugiados das cavernas denunciam a descarga | Vítimas e parentes compartilham a mesma memória indiferenciada |
| Selar a rede | Famílias exigem registro dos cativos; a proteção não encerra o protesto | Vozes antes aprisionadas integram o novo Coração, sem consentimento individual |
| Canalizar o corpo na ponte | Escolta marcada retorna e disputa o culto ao herói | Marcas desaparecem, junto da lembrança de quem decidiu atravessar |
| Romper a ponte | Vigília registra a escolta isolada sem presumir todos mortos | Ponte recomposta; ausência da escolta deixa de ser reconhecida |

| Entregar veneno ao alquimista | Sobreviventes tratados reivindicam cuidado contínuo, mesmo após conter a Bruma | Corpos recompostos perdem a lembrança do tratamento e de seu consentimento |
| Destruir a pesquisa | Lysa preserva uma página vazia para o pesquisador perdido; famílias discutem a cura que não receberam | A página recebe um símbolo uniforme, não um nome |
| Selar a seiva | Oren guarda madeira morta como testemunho do bosque perdido | Árvores renascem iguais, sem os nomes dados pelos antigos moradores |
| Drenar a seiva | Refugiados mantêm mapas das casas presas às raízes | Casas e raízes se fundem; os habitantes esquecem onde terminava seu abrigo |
| Oferecer vitalidade no altar | Cisternas continuam isoladas; gratidão ao herói convive com a água perdida | Água retorna, mas ninguém lembra que o sacrifício teve um autor |
| Executar o cultista no altar | Placas de sentença persistem; o conselho disputa se devem permanecer | As placas deixam de listar julgados e repetem apenas o Coração |
| Libertar cativos do ossuário | Irian conserva retratos para famílias que perderam memórias dos mortos | Retratos exibem o mesmo rosto, completando a dissolução dos nomes |
| Reforçar o selo | Vozes permanecem no arquivo e famílias exigem fim da prisão | As vozes se tornam parte da harmonia coletiva, sem libertação individual |
| Drenar aos níveis inferiores | O abastecimento funciona; trabalhadores ausentes recebem memorial | Túneis se refazem sem a história de quem vivia neles |
| Inverter o fluxo | Lysa mantém ala de sobreviventes e registro da doença da superfície | Janelas se abrem, mas residentes não distinguem suas antigas casas |
| Extrair o núcleo | Oren sela a melhoria como obra única e guarda relatos das máquinas libertas | A peça se integra às novas forjas; autoria e vítimas desaparecem dos registros |
| Estabilizar o núcleo | A máquina continua útil e audível; placas identificam exploradores presos | As vozes da máquina se confundem com as de toda a cidade |
| Perdoar as almas no pedestal | Refugiados externos continuam pedindo abertura dos portões | A cidade os integra sem preservar suas identidades de origem |
| Condenar as almas no pedestal | Estandartes da guarda preservam a doutrina; famílias contestam nomes retidos | O símbolo substitui todos os estandartes; obediência já não parece imposta |

As 22 opções anteriores ao final têm acima uma projeção para cada desfecho; as duas opções do mapa 12 selecionam os finais das seções 9/10. Em produção, catálogo mantém essa cobertura das 24 opções. Contradições são erro de conteúdo, não resolvidas por escolher uma fala ao acaso.

## 12. Contrato de cenas e diálogos para scripts

| Estado | Gate | Efeito de domínio | Evento de apresentação |
|---|---|---|---|
| Prólogo pendente | Nova campanha | Objetivos/checkpoint criados uma vez | Mostrar P01–P03 |
| Hub observado | Stage + flags aplicadas | Nenhuma mutação econômica por falar | Resolver fala/retrato permitidos |
| Final intro | Acesso autorizado | Marcar cena disponível, não conceder loot | F01/F02 e variante de classe |
| Combate ativo | Comando de início validado | Simular HP/MP/vigor/stamina e fases | FCT/HUD/animações confirmadas |
| Vitória confirmada | HP de Vael 0 | Recompensa única e estado awaiting_final_choice | F03 / painel de opções |
| Final escolhido | Uma opção válida | Registrar ending_id, efeitos e campaign_completed | E01 ou E02 + epílogos |

Campos mínimos: scene_id, line_id, speaker_id, class_variant, conditions, portrait_id, text_key, next_node_id e choice IDs quando aplicável. Cada efeito é ID permitido, nunca código executável enviado pela UI. Narrativa em JSON/Resources; serviço de aplicação valida e persiste; EventBus notifica fatos após commit.

Online só enviar texto/flags visíveis. Cenas de revelação não são distribuídas como resposta de tutorial; assets locais podem ser inspecionados pelo dono e não há promessa de segredo inviolável. Autoridade de quest/loot não depende de ocultar o arquivo de roteiro.

## 13. Validação antes da produção

- Prólogo pulado/assistido chega ao mesmo objetivo/checkpoint sem repetir equipamento.
- Cinco classes têm fala de Vael, resposta e reação; tratamento não contradiz a linhagem.
- Ferreiro, alquimista e mercadora têm variantes por estágio, fatos e empate; não há spoilers antecipados.
- Cada final respeita quest flags e nomes canônicos; restore_order não vira ruptura do filtro.
- Morte/retry não revela vitória, repete custos ou concede receita/fragmento.
- Gates de fase 65%/30% são únicos e processam saltos de HP sem sobreposição perigosa.
- Loot confirmado precede escolha; reload no painel não abre outro encontro.
- HUD/controle retomam somente no estado correto; hub permanece seguro.
- Catálogo futuro cobre ambas as opções das 12 quests em ambos os finais, com teste de consistência das combinações.

Este módulo entrega roteiro principal completo e diretrizes de encenação, com epílogos condicionados. Não entrega cutscenes executáveis, diálogos carregáveis na engine ou boss balanceado. Próximo marco: produzir scripts e cena mínima Godot, validar o fluxo offline e ampliar o catálogo narrativo conforme estes contratos.
