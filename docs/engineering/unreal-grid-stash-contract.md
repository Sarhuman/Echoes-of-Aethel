# G21 — contrato de grade, baú e migração de inventário

**Estado: contrato C0, 15/10/2026. Sem runtime Unreal.** Implementa a direção [D01](diablo-direction-decisions.md) sem alterar a bolsa Godot. Os tamanhos de itens, dimensão final da grade e capacidade do baú são `EXPERIMENTAL` até catálogo/economia simulados; 10 × 6 é apenas o primeiro layout a medir. Não confundir a grade visual já desenhada no HUD Godot com itens de largura/altura.

## Identidade e propriedade

- Cada item de equipamento tem `instance_id` estável e único, `template_id`, versão de conteúdo, propriedades/afixos validados e `owner_character_id`. Consumível/material tem pilha com `instance_id`, `template_id` e quantidade 1–20. Não usar nome traduzido, posição na grade ou caminho de asset como ID.
- A propriedade de uma instância é **uma** destas localizações: bolsa, baú **do mesmo personagem/campanha**, reserva pendente ou outra localização explicitamente definida. Equipamento vestido é referência ao `instance_id` da bolsa, não cópia; ao mover a peça para baú, deve desequipar ou recusar conforme regra de UI aprovada.
- Ouro, XP, receitas e fatos de quest ficam em campos próprios. Item de quest essencial é um fato persistido ou item em compartimento de quest sem limite espacial; nunca pode ser perdido por grade cheia.
- Baú compartilhado entre personagens é **D09 em aberto**. Não implementar transferência intercampanha por inferência deste contrato.

## Grade e transações

- Templates versionados declaram `width_cells`, `height_cells`, `max_stack`, família e slots válidos. Dimensões inteiras positivas, dentro da grade; rotação desligada na primeira versão. Itens não podem se sobrepor nem sair dos limites. Cada placement guarda `instance_id`, `x`, `y`; a ordenação visual não altera propriedade.
- Arrastar, empilhar, dividir pilha, equipar, comprar, vender e guardar no baú começam como **comandos candidatos**. O domínio valida catálogo, propriedade, espaço, ouro, quantidade e versão de save; grava snapshot íntegro; só depois UI e VFX confirmam. Recusa mantém estado e recursos. Repetir `request_id` confirmado devolve resultado anterior.
- Ao conceder vitória/baú/quest, `reward_id`, seed/resultado de loot, instâncias, XP, ouro e fatos mudam no mesmo commit. Primeiro tentar encaixe determinístico em pilhas compatíveis e depois em espaço livre; sem encaixe, registrar instância na **reserva persistente** antes de marcar o prêmio como concedido. Limite inicial de 100 instâncias na reserva vem do GDD 03; sem capacidade, impedir iniciar uma concessão potencial que exceda o limite ou prover retirada segura antes do encontro. Nunca deixar o item apenas no chão ou na memória após confirmar `reward_id`.
- Salvar dimensões do template ou versão que permita reconstruí-las. Na carga, validar não sobreposição, IDs únicos, quantidade, slot equipado, propriedade e referência ao catálogo. Conteúdo incompatível não vira item silenciosamente: falha controlada com backup ou migração versionada.

## Migração eventual do Godot

O `InventoryModel` Godot atual guarda contagem por `item_id`, ouro e dois slots equipados; capacidade de 60 **posições** e peso máximo são regras daquele cliente. Um conversor Godot→Unreal só será oferecido depois de schema Unreal versionado e testes. Ele lê **cópia** do save Godot validado, preserva o original, cria `instance_id` determinísticos para o plano de migração, divide contagens em pilhas legais, resolve `template_id`, atribui dimensões do novo catálogo e posiciona por ordem estável. Excedentes entram na reserva; equipado aponta a uma instância existente. IDs de quest, decisões, ouro e XP não podem ser duplicados ou perdidos. Falha em qualquer etapa aborta o novo save, deixa o original intacto e registra erro recuperável. Reexecutar a mesma migração não cria novos itens ou recompensas.

## Aceite antes de UI/baús Unreal

1. Testes de encaixe: bordas, sobreposição, item maior que grade, pilhas cheias/parciais, drag inválido e rotação recusada.
2. Testes de propriedade: mover/vestir/desequipar/guardar sem duplicação; dois personagens/campanhas não compartilham instâncias; troca recusada não cobra ouro.
3. Testes de recompensa: grade cheia, reserva cheia, crash antes/depois do commit, retry de morte/baú/conversa, duas concessões com IDs distintos e recuperação do backup.
4. Testes de migração em cópias: 60 posições Godot, peso, stacks de 20, equipamento, ouro, quest e excedente. Comparar multiconjunto de itens e saldos antes/depois; nenhum save real é sobrescrito.

Ver também [persistência Unreal](unreal-persistence-contract.md), [GDD 03](../GDD/03_sistema_de_loot.md) e [auditoria G02](g02-code-portability-audit.md).

