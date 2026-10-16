# Agregado de campanha e progressão C# — 16/10/2026

**Estado:** incremento executável headless, fora de Unity. Fontes em `shared/csharp/Aethel.Campaign/`. Não há cenas, monstros, mentor, tela de árvore, efeitos de especialização ou cliente Unity criados por esta entrega.

## Uma gravação para a transação

`CampaignState` contém identidade de personagem/classe/campanha, nível confirmado, ouro, reserva de poções, especialização, aviso e histórico de comandos, junto com `MemorySave`. Gravações também recusam apagar decisões, pistas, marcos revelados ou recibos já confirmados. A versão de memória é a versão do agregado; qualquer comando de progressão também a avança. Não existem versões independentes que possam aceitar dois estados incompatíveis.

`CampaignMemoryAdapter` implementa a interface já usada por `MemoryService`. Esse serviço continua validando pistas/rotas/escolhas. O adaptador integra a recompensa do Bosque antes de enviar o candidato inteiro a `ICampaignStore.TryCommit`. Em conflito ou falha de gravação não confirma escolha, recompensa ou carga ritual isoladamente. A carga principal é derivada do número de escolhas canônicas únicas, sem segundo contador editável; secundárias não entram nesse mapa de escolhas.

Recompensa portada de `QuestDefinition`/`QuestService` Godot: `judge_suspects` concede 25 ouro uma vez; `shelter_villagers` reserva três `potion_health_small` uma vez. **Limite explícito:** ainda não há inventário/peso C#, então as poções ficam em `ReservedHealthPotions`, não são anunciadas como itens utilizáveis ou inseridas em uma bolsa inventada. Entrega da reserva precisa ser integrada atomicamente ao futuro inventário, com limite de capacidade e replay. Recompensas de vitória dos bosses e das outras onze quests não foram criadas por inferência.

O formato de protótipo é novo (schema 1 com campos obrigatórios de personagem), exclusivo do agregado. Não substitui save Godot nem importa automaticamente o JSON isolado do PR #26. Depois de iniciado, identidade de personagem/classe/campanha deve corresponder ao store. Corrupção e identidade incompatível geram erro; não começar silenciosamente uma campanha vazia ou restaurar backup apagando decisões confirmadas.

## Alocação e respec

`ProgressionService` implementa:

- Nível confirmado entre 1 e 30, crescente. `SetConfirmedLevel` é ponto de integração com o futuro serviço de XP confiável; NÃO calcula XP e não deve receber valor arbitrário da UI.
- Primeiro ponto no 5, depois 10/15/20/25/30, inclusive subida múltipla ou seleção atrasada. Consulta não gasta pontos.
- Primeira direção no nível 5, com inicial gratuito; direção secundária só no 10. Quatro tiers por direção exigem anterior da cadeia, níveis mínimos 5/10/20/30 e orçamento total máximo de seis nós.
- Dispensa de aviso persistida separadamente da seleção. Dispensar não bloqueia escolher; seleção independe do popup.
- Contexto de treinamento por `ITrainingAccess`, fornecido pelo adaptador confiável que verifica hub seguro, NPC habilitado, classe, combate e ações pendentes. Não há implementação de cena desse acesso; usar permissão permanente em produção invalidaria essa proteção.
- Primeira redistribuição confirmada gratuita; seguintes custam `5 × nível` em ouro. No-op não cobra e não consome gratuidade. Candidato inválido, ouro insuficiente, contexto recusado, conflito ou falha de persistência preservam o estado.
- Comando UUID normalizado, vinculado ao payload confirmado. Replay do mesmo comando não redistribui/cobra novamente; mudar payload conflita. Ordem dos nós em respec é normalizada, duplicatas são recusadas.

IDs de classe continuam canônicos. **NOVOS/EXPERIMENTAIS:** IDs técnicos de direções deste protótipo são `<classe>.defense/rupture`, `.control/burst`, `.precision/ambush`, `.command/sacrifice`, `.protection/judgment`, conforme a classe; nós acrescentam `.t1` a `.t4`. Eles não existiam nos saves anteriores e não são nomes narrativos de subclasses. Qualquer mudança após persistência exige migração, não renomeação silenciosa.

**Alocação não é efeito de combate:** os quarenta nós estruturais das cinco classes servem para testar orçamento/cadeia/persistência. Nenhum tier posterior nem os dois modificadores iniciais do Bastião executam efeitos aqui. Não conectar compra a um jogo público enquanto seu conteúdo/efeito não estiver implementado. Não há compra das habilidades básicas, atributos, HP/MP, cooldowns ou invocações neste agregado; integrar esses dados antes de afirmar respec sem exploits no gameplay completo. Este módulo não reinicia esses sistemas porque ainda não os possui.

## Persistência e limites operacionais

`JsonCampaignStore` faz CAS sob arquivo de lock, temporário no mesmo diretório, flush e substituição com backup. O armazenamento é acessado de novo após a transação para apresentar estado confirmado. Arquivo abandonado `.tmp` não vira autoridade. O candidato recebe cópia profunda; leitura pública não oferece referências ao estado interno.

Limites atuais: arquivo até 1 MiB e até **512 recibos de comandos de progressão** por campanha. Não usar esse histórico para ataques, movimento ou eventos de alta frequência. Ao atingir o limite, novas operações são recusadas com `HistoryFull`; nada é descartado silenciosamente. Antes de produção, definir manutenção/migração do histórico com janela de retry explícita. Essa limitação é uma pendência de produto, não capacidade ilimitada de save. Não é proteção anticheat offline.

O consumidor precisa tratar exceções de leitura/arquivo bloqueado/JSON inválido com mensagem e recuperação explícita. O status de falha de persistência cobre a tentativa de commit; não esconde uma falha ao ler o estado confirmado. Concorrência entre processos, falta de energia, permissões e recuperação assistida devem ser testadas em Windows antes de aceite de produção.

## Ameaça e conteúdo regional

`ThreatPresentation.Compare` projeta níveis 1–30 e categoria confirmados em banda, papel de cor, texto acessível, símbolo e barra de boss. Diferença +4 ou maior: vermelho escuro; +1…+3: vermelho claro; 0: branco; negativa: azul. Elite/subchefe/boss mantêm categoria independentemente da banda. O serviço não executa percepção nem deve receber inimigos invisíveis por conveniência da UI. Não altera atributos, loot ou nível.

`CampaignContent.cs` é gerado de `regional-threat-and-training.md`, manifesto e definição Godot da recompensa. As faixas são recomendações experimentais de catálogo, sem spawn ou autoscaling. Hubs/IDs desconhecidos não recebem nível hostil. O Coração não possui faixa de comuns. A regra das duas dungeons permanece design até catálogo próprio; não criar IDs de viagem a partir de nomes editoriais.

## Validação reproduzível

Ambiente local: Mono compiler/runtime 6.8.0.105, C# 7.2, warnings tratados como erros. Dependências: `mono-mcs` e `libmono-system-runtime-serialization4.0-cil`; sem pacotes NuGet no domínio.

```bash
python3 tools/generate_world_memory_catalog.py
python3 tools/generate_campaign_content.py
bash tools/run_world_memory_tests.sh
bash tools/run_campaign_tests.sh
python3 -m unittest discover -s tools/tests -q
```

O novo harness cobre matriz de 7.500 alocações das cinco classes/níveis, 3.600 comparações de ameaça, falhas/replay/CAS, limite de histórico, respec grátis/pago, recompensa em agregado, overflow de ouro, identidade, arquivo temporário, backup, reload e uma campanha completa de doze decisões composta com especialização. A suíte anterior continua cobrindo 4.096 rotas de escolhas. As muitas asserções de matriz não equivalem a funcionalidades ou playtests.

Windows CI recebe `dotnet run --project shared/csharp/Tests/CampaignHarness.csproj --configuration Release`. O projeto .NET 8 é APENAS harness de ferramentas, não versão ou dependência da Unity. Não foi executado localmente neste ambiente; consultar o job remoto antes de declarar compatibilidade Windows. Não há Unity Editor disponível ou build Windows do jogo validada nesta entrega.

## Próximo aceite concreto

Validar U0 (editor/pacotes/cena mínima/build de inspeção); configurar assemblies reais; integrar XP, inventário, recursos e efeitos no mesmo agregado; implementar contexto de treinamento e UI. Somente então testar movimento/combate/mentor e um retorno com consequência visível. O [estado de produção](production-status.md) registra o que falta sem transformar documentação em gameplay concluído.
