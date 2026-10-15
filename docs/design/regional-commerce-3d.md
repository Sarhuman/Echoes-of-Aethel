# Comércio regional e equipamento por classe — proposta 3D

**Estado:** `NOVO/EXPERIMENTAL`, para revisão e simulação econômica antes de criar lojas na Unreal. O cliente Godot vende apenas `potion_health_small` por 10 ouro e `flask_aether_small` por 12 ouro na Cidadela; [GDD 05](../GDD/05_combate_e_hud.md) marca os preços como experimentais. [GDD 03](../GDD/03_sistema_de_loot.md) define famílias/slots, raridades e drops, mas ainda não fecha catálogos de loja dos atos II–IV. Este documento responde como o herói se equipa sem depender de sorte e evita quatro lojas idênticas.

## Regra de vitrine

Cada hub seguro tem **três camadas de oferta**. Uma camada comum garante preparação básica; uma vitrine da classe escolhida mostra equipamento e suprimentos compatíveis com suas habilidades; uma camada regional expressa os materiais e perigos do ato. A vitrine não sorteia um novo inventário a cada abertura, morte ou reload. A campanha usa uma classe fixa; outro herói em outra campanha vê sua vitrine correspondente. Itens de outras classes podem existir no mundo, mas não ocupam a oferta mínima garantida do herói ativo.

| Camada | Conteúdo previsto | Visibilidade e persistência |
|---|---|---|
| Suprimentos comuns | Vida e Éter em quantidade/nível apropriados, antídoto onde o risco o justifique; serviços essenciais | Sempre disponíveis em hub seguro; preço/estoque final por ato vêm de catálogo e simulação. Consequências narrativas mudam falas e bancas, não removem a única cura/compra. |
| Preparação da classe | Ao menos uma opção determinística de arma/foco funcional e uma proteção compatível por patamar de ato, com requisito de nível claro | Filtrar por `class_id`, slot, família, nível e habilidade implementada. Oferta desbloqueia pelo avanço da campanha, não por drop aleatório. Não vender equipamento que o personagem não consegue usar mecanicamente. |
| Especialidade regional | Temas de resistência, materiais e aparência próprios do bioma; uma escolha de equipamento ou utilidade distinta da Cidadela | Desbloqueio por hub/quest observada; a oferta não promete poder melhor que boss/dungeon por padrão. Templates e preços ainda são pendentes. |

**O que muda entre cidades:** os dois frascos atuais podem reaparecer como suprimento básico, mas não compõem o catálogo inteiro. Dunas favorecem proteção contra travessia/calor e peças de caravana; Refúgio oferece materiais de contenção/veneno e equipamento adaptado a canais e máquinas; Posto do Limiar oferece preparação para o Pináculo e resistência apropriada. Esses são **temas**, não afixos, estatísticas ou itens novos aprovados. A Cidadela mantém a oficina, cura e compras iniciais; seus estágios de degradação alteram aparência e diálogo, não retiram serviços.

## Vitrines de cada herói

Os exemplos abaixo especificam **famílias e funções**, sem inventar `item_id`, dano, preço ou drop. Antes de implementar, publicar templates, nível de item/requisito, modelo, animação, custo e fonte. A árvore de habilidades e as restrições de arma do [GDD 01](../GDD/01_classes_e_atributos.md) prevalecem.

| Classe | Oferta determinística relevante | Evitar na vitrine garantida |
|---|---|---|
| Bastião `twilight_bastion` | Machado de uma mão e escudo, com alternativa defensiva de peitoral; arma apoia o básico gratuito | Machado de duas mãos que invalide a Guarda sem explicação; espada sem animação implementada |
| Tecelão `rune_weaver` | Cajado/foco que fortaleça ataque rúnico e proteção leve; recurso de Éter | Foco secundário vendido como compatível com cajado de duas mãos |
| Sentinela `shadow_sentinel` | Adaga e proteção leve para mobilidade; segunda adaga somente com suporte de off-hand | Arco/besta enquanto combate à distância não tiver habilidades/animações validadas |
| Invocadora `thorn_summoner` | Cajado/foco de INT/FTH e proteção que não atrapalhe invocações | Equipamento que prometa invocação extra além do limite do GDD |
| Inquisidor `sacred_flame_inquisitor` | Maça e proteção adequada ao combate físico/sagrado | Efeito de cura ou santidade sem teto/cooldown definido |

No slice Godot, [D8](../engineering/decisions-first-slice.md) limitou equipamento a `main_hand` e `chest`; o GDD 03 prevê nove slots para a campanha. A transição 3D precisa de decisão de escopo e catálogo antes de expor escudo/off-hand e os demais slots na loja. Esta proposta **não** declara que o cliente Unreal já suporta qualquer slot. Para cada classe, a primeira oferta deve acompanhar o ataque gratuito e pelo menos uma defesa, inclusive se o sistema completo de afixos for adiado.

## Identidade dos quatro mercados

| Hub e comerciante | Identidade visual/narrativa | Oferta a catalogar |
|---|---|---|
| Cidadela `citadel`: Sera e Lyra, Bram para futura forja | Banca de abastecimento, enfermaria e oficina no abrigo seguro; estoque responde em fala à aldeia/caravana | Frascos existentes com preços atuais experimentais; kit básico de cada classe e reparo/forja **somente após contrato próprio** |
| Acampamento `dunes_camp`: vendedor local ainda sem nome/ID | Feira de lona, peças recuperadas de carroças, sal e água contados; não transformar nômades em inimigos por identidade | Suprimentos comuns, proteção de travessia e alternativa de arma do ato II por classe |
| Refúgio `underground_refuge`: vendedor local ainda sem nome/ID | Bancada seca sobre canais, peças reaproveitadas e avisos sobre água contaminada | Suprimentos comuns, antídotos e equipamento apropriado para esgoto/fundição por classe |
| Posto `liminal_post`: responsável local ainda sem nome/ID | Última estação antes do Pináculo; registro de nomes e objetos dos que não voltaram | Preparação final de cada classe, suprimentos e acesso à reserva; sem lendário obrigatório vendido antes de Vael |

Sera pode aparecer por narrativa em atos posteriores, mas **não** presumir que viaja fisicamente para todos os hubs. Um vendedor local ou representante com identidade própria mantém o serviço. Os três comerciantes locais, suas falas, lojas e IDs são lacunas editoriais, não NPCs canônicos já existentes. Um hub não oferece botão de compra antes de ter ator/serviço, catálogo válido, preço e save testados.

## Regras econômicas e de UI antes de implementação

1. **Acesso justo:** testar cada uma das cinco classes com drops mínimos plausíveis. O ouro obtido na rota principal deve permitir suprimentos e uma atualização essencial por ato; dungeon opcional melhora opções, não corrige um bloqueio econômico obrigatório. Nenhuma escolha moral aplica aumento oculto de preço ou diminuição permanente de drops.
2. **Preço e estoque:** o GDD só fecha 10/12 ouro para dois frascos iniciais. Valores dos demais produtos, recompra e estoque finito continuam pendentes. Não interpolar preço automaticamente pelo nível do herói. Simular ouro/hora, custo de cura, mortes e compras antes de aprovar cada região.
3. **Transação:** compra tem `request_id` estável, valida hub, saldo, bolsa, compatibilidade e versão; ouro e item são confirmados juntos. Retry mostra a mesma compra; falha não gasta ouro. Vender item único/quest/recompensa ou consumir material de missão exige política específica. Exibir comparação de equipamento, requisito, slot, preço, ouro restante e confirmação; preço não fica escondido atrás de cor de raridade.
4. **Loot preserva valor:** as probabilidades/raridades de comuns, elites, subchefes e bosses continuam no GDD 03. A loja garante preparação básica, mas não comercializa fragmentos, receitas únicas, recompensas de decisão ou prêmio exclusivo da primeira conclusão de dungeon. Não vender uma cópia que esvazie o significado da exploração.
5. **Acessibilidade:** mostrar raridade por rótulo e moldura além da cor; permitir inspeção sem compra, texto ampliável e navegação por teclado/controle. A interface lê fatos confirmados, não cria itens nem anima barras para efetuar débito.

## Para fechar esta proposta no gate C0

Criar e revisar uma planilha/catálogo por ato com cada `item_id`, classe permitida, família, slot, `item_level`, `requirement_level`, estatísticas, preço de compra, política de venda, fornecedor, desbloqueio e asset. Simular cinco classes em rota principal, sem drops favoráveis; publicar critérios e resultados. Reconciliar os nove slots do GDD com D8 antes de aprovar a vitrine. Só depois produzir bancas, retratos, widgets e transações Unreal. O [registro de conteúdo](../engineering/campaign-content-ledger.md) acompanha essa lacuna.
