# Direção Unity e campanha completa — 16/10/2026

**Direção solicitada pelo proprietário nesta conversa:** preparar um cliente limpo Unity para o ARPG 3D, preservando história, contratos e referências úteis. URP é a opção de pipeline proposta para o PC informado (Xeon 2630, RX 580, 16 GB). A versão exata do editor e dos pacotes será fixada após verificar a instalação local e executar uma cena mínima; este documento não afirma que existe projeto Unity compilado.

Esta decisão substitui Unreal como destino de produção e as instruções específicas de sua engine. Não muda IDs, opções, fragmentos, mortes, recursos ou finais. Godot continua a evidência histórica de jornada jogável; Unreal contém infraestrutura e blockouts. Nenhum deles foi apagado, migrado automaticamente ou convertido em C# neste PR.

## Ordem e autoridade

1. Este documento rege engine, isolamento do novo cliente e entrega atual de revisão de campanha.
2. `production-decisions.md` e `decisions-first-slice.md` continuam regendo domínio; `campaign-route-manifest.json` conserva os 12 mapas e 24 opções.
3. `campaign-interaction-spec.json` e sua versão legível `campaign-interactions.md` são propostas editoriais de interações para TODOS os mapas, quatro hubs, três secundárias e duas dungeons. Nomes/encenação novos não estão automaticamente aprovados.
4. GDD 04 rege consequências exatas e seus gatilhos; não substituir efeito tardio por reação instantânea. IDs dos finais não mudam. Títulos de produção prevalecem sobre o resumo antigo corrigido neste PR.
5. O C0 continua aberto. A revisão de cobertura pode avançar em todas as regiões agora; isso não comprova aprovação visual ou gameplay. A proposta de limitar o bloqueio de C0 ao detalhamento da primeira rota precisa de decisão explícita antes de iniciar gameplay. Não iniciar doze cenas funcionais para satisfazer quantidade documental.

## Uma única base ativa, com preservação

O novo cliente ficará em `client/unity/`, sem dependência de execução de GDScript, C++ Unreal, .tscn ou .uproject. Antes de remover clientes antigos da árvore ativa, o Codex local deverá:

- Conferir alterações não commitadas, branches e PRs #22–#24; não descartar trabalho ou fazer merges automáticos.
- Registrar commits de referência e criar arquivo/tag de preservação com autorização aplicável; verificar recuperação inclusive de objetos LFS. Uma tag não substitui os objetos LFS.
- Separar contratos de comportamento de APIs da engine. Preservar textos, IDs, conceitos e casos de teste válidos; não copiar a projeção 2:1 do Godot como física 3D.
- Atualizar documentação vigente e links quando houver de fato um cliente Unity inicializado; retirar ferramentas antigas apenas em PR de limpeza específico, com backup verificado.

## Arquitetura de implementação proposta

| Camada | Responsabilidade | Limite |
|---|---|---|
| Domain (C#) | Dano, recursos, inventário, decisões, recompensas e invariantes | Sem MonoBehaviour, cenas, UI ou áudio |
| Application (C#) | Comandos, validação, versões e transações | Interface não confirma a própria recompensa |
| Content | ScriptableObjects/catálogos confiáveis, localização, IDs e referências | Arquivo de save não carrega scripts/assets arbitrários |
| Infrastructure | JSON versionado, escrita atômica, backup, migração e RNG injetado | Só publicar fato após confirmação |
| Unity adapters | Input, movimento, física, navegação, transição de cena | Traduzir intenção/resultados, sem duplicar domínio |
| Presentation | HUD, câmera, animação, áudio, VFX | Não conceder dano, XP, itens ou decisões |

Assembly definitions e testes EditMode/PlayMode serão criados pelo cliente real, com versões registradas, não como arquivos de aparência válida sem editor. Versão Unity, URP, input e navegação devem constar dos manifests/lockfiles gerados e revisados. Evitar arquitetura com rede/contas antecipadas. Save Godot não será sobrescrito pelo novo cliente; schema/identidade próprios e conversor somente com migração testada.

## Orçamento do PC

Não habilitar pipeline de alto custo por aparência. Medir abertura fria/quente, compilação, CPU/GPU/frame time, RAM e disco com hardware exato. Começar com um blockout pequeno, luzes limitadas e materiais simples. Qualidade desejada em screenshot não comprova desempenho. Não prometer FPS para a RX 580 sem identificar VRAM, revisão da CPU, resolução e medir. A prova de renderização da Unity não exige produzir todos os atores ou sistemas da campanha.

## Próximas entregas verificáveis

- U0 — registrar editor/pacotes locais, abrir cena mínima e gerar build Windows de inspeção. Aceite: abre fora do editor, registra resolução/hardware e não contém gameplay alegadamente pronto.
- C0 editorial — revisar propostas deste pacote, aprovar ou recusar novos nomes e interações, fechar IDs e conflitos. Métrica: completude por aceite, não porcentagem genérica de jogo.
- C0 visual — câmera/escala em Unity, um piloto completo de Bastião e estimativa de custo por ator/mapa. Adaptar medições anteriores sem depender de .umap.
- U1 após gate — Bastião, trecho do Bosque, Lobo e combate básico com testes de domínio e física. Testar no PC antes de expandir.
- U2 — completar primeira rota, quest, equipamento, recompensa/save, retorno e Garras; build e playtest humano.
- U3 — Pântano e Portal com suas interações próprias; não copiar a investigação do Bosque.
- U4 — ato II e Coroa de Sucata, depois Livro dos Nomes, seguindo dependências de zona. Dungeons nunca obrigatórias para progressão principal.
- U5 — atos III/IV e demais classes como incrementos aprovados; elenco completo descrito não é autorização para produzir tudo simultaneamente.

## Incremento de domínio — 16/10/2026

Após a solicitação de implementação, foi criado um [núcleo C# de memória do mundo](world-memory-implementation.md) isolado de engine, compilado e testado em Mono. Ele reutiliza as escolhas e marcos canônicos e possui uma prova de persistência própria; não equivale ao cliente Unity ou ao agregado completo de quests/save. Integração de recompensa/ritual na mesma transação e validação no editor continuam pendentes. C0 e U0 não foram declarados concluídos.

## Registro de lacunas real

| Lacuna | Entrega desta revisão | O que ainda fecha o aceite |
|---|---|---|
| Atenção concentrada no Bosque | Fichas comparáveis dos 12 mapas | Plantas, interações implementadas e playtest |
| Hubs II–IV sem pessoas próprias | Papéis, nomes propostos e memória regional | Aprovação editorial, IDs, diálogo final e atuação |
| Dungeons superficiais | Sequência de descoberta, combate, retirada, retorno e recompensa | Catálogo, layout, modelos, boss e testes na engine |
| Secundárias sem local/função | Localização proposta e separação da quest principal | Aprovação de IDs, roteiro final e orçamento |
| Inconsistência no título de final | Resumo alinhado às decisões de produção | Revisar localization/cenas futuras contra os mesmos IDs |
| Transição confusa de engine | Precedência Unity e fronteiras de arquitetura | Projeto Unity instalado, versões fixadas, build |
| Loot e economia | Papéis por região e limites de recompensa preservados | Instâncias/afixos/catálogo, preços e simulações por classe |
| Conteúdo ≠ funcionamento | Validador de cobertura e testes negativos | Regressões C#, save, integração e teste humano |

Não chamar este pacote de “todas as lacunas fechadas”. Ele fecha a especificação inicial de cobertura e torna o restante explícito. Não há novos mapas Unity, modelos 3D, balanceamento aprovado ou build nesta entrega.
