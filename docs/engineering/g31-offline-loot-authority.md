# G31 — autoridade de loot no jogo offline

**Estado: reconciliação de especificações C0, 15/10/2026. Sem serviço de loot Unreal implementado.** O [GDD 03 §4–5](../GDD/03_sistema_de_loot.md) descreve DTOs FastAPI, PostgreSQL/JSONB, servidor autenticado e RNG online. A direção atual é jogo solo offline em Unreal, com multiplayer adiado por [D08](diablo-direction-decisions.md). O [contrato de persistência](unreal-persistence-contract.md) rege a implementação local; isto não muda chances, raridades, itens ou XP do GDD.

| Exigência do GDD 03 | Aplicação offline Unreal |
|---|---|
| Servidor valida vitória e propriedade | Serviço de domínio/aplicação local valida evento confirmado de encontro, personagem/campanha, catálogo e versão do estado. Actor, Blueprint, HUD e efeito visual não concedem loot. Não chamar isso de anticheat ou autoridade remota. |
| Chave única `(character_id, campaign_id, reward_id)`; morte comum por `(encounter_id, spawn_instance_id)` | Índices lógicos no save versionado; o resultado original da requisição fica persistido para retry. Nova expedição só gera nova instância quando sua política permitir. |
| Transação SQL com rollback | Construir candidato completo em memória e validar; gravar envelope temporário íntegro, sincronizar/fechar e substituir o save com backup; publicar novo estado em memória só após sucesso. Falha não concede parcialmente. Testar interrupção antes/depois da substituição. Implementação de arquivo/rename deve respeitar a plataforma e comprovar atomicidade prática no Windows. |
| JSONB de `character_inventories`, versão otimista e bloqueio de linha | Schema local versionado com um escritor por campanha, número de revisão e recusa de comando baseado em revisão antiga. Não portar SQL, locks de banco nem DTOs FastAPI para C++. |
| RNG seguro online; seed não vem do cliente | RNG do domínio local injetável em testes. Registrar seed e resultado **no commit da vitória**, sem rerrolar no reload/retry. O jogador controla o dispositivo; checksum/backup detectam corrupção, não impedem adulteração intencional. Não prometer segurança online em modo offline. |
| Template, raridade, afixos e projeção final de stats validados | Preservar IDs e catálogo versionado. `Stats` derivado é projeção de template+afixos, nunca somado duas vezes. Antes de sortear, validar pool, pesos inteiros e compatibilidade de classe/nível/slot. Afixos permanecem adiados na primeira rota conforme o plano; não gerar instância incompleta como se estivesse pronta. |
| Bolsa cheia usa reserva; primeira vitória de boss concede receita/fragmento uma vez | Preservar exatamente, agora com [grade e baú](unreal-grid-stash-contract.md). A reserva é persistida na mesma revisão que XP/ouro, item e `reward_id`; a conversa/cinemática só apresenta o resultado. |

**Fronteira de confiança:** save local com hash e backup protege recuperação de falhas acidentais, não é prova de que o usuário não alterou arquivos. Não adicionar backend, conta, sincronização em nuvem ou bloqueio remoto para resolver um risco que o produto offline não promete resolver. Se multiplayer for reaberto, redesenhar autoridade e migração; não reutilizar este contrato local como servidor.

**Aceite G31:** os pontos de autoridade, idempotência, RNG, transação e legado online acima estão decididos no documento. A implementação futura exige testes de duplicação, falha de gravação, backup, bolsa/reserva cheia e catálogo inválido antes de recompensas jogáveis. G31 documental pode ser marcado concluído; G21 e gameplay permanecem pendentes.

