# Edição de sprites com Aseprite

> **Escopo Godot/arte 2D:** workflow histórico de sprites/tiles. Não rege geometria, navegação ou animação do mundo Unity 3D. Seguir [guia visual Unity](../design/unity-art-direction.md); aplicar à UI 2D somente quando apropriado.

Aseprite é a ferramenta de edição para os próximos sprites, ícones e animações em pixels. Salve o arquivo-fonte `.aseprite` no repositório e exporte as sprite sheets `.png` usadas pelo Godot. Registre dimensões do quadro, origem, ordem dos quadros e nomes das animações; mantenha transparência real e confira o alinhamento do personagem em movimento, ataque e guarda.

Os PNGs pintados atuais são assets originais gerados para a fatia jogável; não existem fontes `.aseprite` para eles. A proveniência está em `docs/media/runtime-art.md`. Uma edição futura no Aseprite deve acrescentar o fonte correspondente e atualizar esse registro. A referência de combate atual é Path of Exile 2; não reutilize seus sprites, nomes ou interface.

Ao trocar uma folha de sprites, capture no jogo pelo menos caminhada, ataque e inventário. Confira que a animação corresponde ao hitbox e que os ícones continuam legíveis na interface.
