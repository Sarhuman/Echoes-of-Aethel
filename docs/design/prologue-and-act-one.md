# Prólogo e Ato I

O [módulo 07](../GDD/07_roteiro_e_dialogos.md) consolida prólogo, conselho, diálogos dinâmicos e roteiro final. Em conflitos de encenação/fala, prevalece o 07; padrões de encontros do Ato I abaixo continuam baseline experimental.

Texto de produção. Falas exibidas não revelam a linhagem; observações de consequência pertencem à implementação e ao diário após o evento.

## Abertura — O cristal que ainda respira

1. Tela escura, vento e três batidas metálicas. Um cristal pulsa sobre a silhueta da Cidadela. Legenda: “Quando o céu se fechou, os sinos continuaram tocando. Já não chamavam os vivos.”
2. A câmera desce ao portão. O herói chega sozinho; seu rosto aparece no retrato 64×64. O guarda examina uma marca luminosa na mão e recua meio passo.
3. Maera, responsável pelo portão: “Guarde essa mão. Aqui dentro, qualquer brilho vira promessa.” O controle retorna ao jogador; objetivo: procurar Irian junto ao cristal.
4. Irian, guardião do arquivo: “A névoa não é natural. Ela respira com o sangue de Silvano. Não confunda silêncio com caminho livre.”
5. O cristal responde à aproximação do herói. Irian interrompe o gesto: “Há pedras que lembram de quem nunca vimos. Não toque.” A câmera não mostra símbolos que confirmem Vael.
6. Objetivo: sair para o Bosque e investigar os desaparecimentos na estrada. Tutorial de movimento, interação, inventário e cura acontece em área segura. Ao sair, registrar checkpoint e mostrar nome da zona.

A cena pode ser pulada. Pular ainda registra objetivos e checkpoint uma vez; o diário permite reler falas. Nenhuma recompensa depende de assistir à cutscene.

## Respostas de chegada por classe

| Classe | Resposta a Maera |
|---|---|
| Tecelão | “Não vim prometer. Vim fechar o que minha ordem abriu.” |
| Sentinela | “Se encontrarem uma cura, deixo o brilho com vocês.” |
| Bastião | “Conheço estes portões. Foi o lado de fora que mudou.” |
| Invocadora | “Os que vieram comigo não precisam de abrigo. Eu preciso.” |
| Inquisidor | “Já segui luzes que mentiam. Esta também terá de responder.” |

## NPCs e deterioração

| NPC / função | Antes | Após acolher a aldeia | Após executar os suspeitos |
|---|---|---|---|
| Maera / portão | “Traga nomes. Estamos cansados de contar sombras.” | “Vieram vivos. Hoje isso precisa bastar.” | “A estrada abriu. Ninguém quis celebrar.” |
| Oren / ferreiro | “Aço não promete. Só precisa aguentar mais um golpe.” | “O aprendiz adoeceu. Eu cuido da oficina.” | “Trouxeram espadas sem dono. Não vou vendê-las.” |
| Lysa / alquimista | “Se a água mudar de cheiro, venha antes da sede.” | “Fervemos tudo. Mesmo assim, o gosto permanece.” | “Tenho remédios sobrando. Isso nunca foi boa notícia.” |
| Irian / arquivo | “Nem toda lembrança pertence a quem a carrega.” | “A marca do cristal apareceu no poço. Copiei antes que apagassem.” | “As pedras da estrada repetem o mesmo nome. Nenhum viajante o disse.” |

Serviços seguem disponíveis e o hub nunca causa dano. Retratos e cenário mostram a mudança; falas não expõem a causa oculta antes de observação suficiente.

## Mapa 01 — A Caça ao Espião

Mensageiro: “Eles davam comida aos homens de túnica. Vi com estes olhos. Se chegarem aos portões, vão abrir para os outros.”

Moradora: “Demos pão a quem veio armado. Meu menino está febril há dias. Olhe para ele antes de decidir o que somos.”

Investigação acessível antes da escolha: pegadas de cultistas passam pela aldeia; parte dos relatos diverge; o poço local tem brilho violeta. Registrar esses fatos, sem diagnóstico definitivo.

Opções: **“Executar os suspeitos e proteger a estrada.”** / **“Escoltar os moradores até a Cidadela.”** Confirmação explica irreversibilidade no save, sem antecipar resultado. Efeitos e retorno seguem a campanha canônica.

## Encontros do Ato I — valores experimentais

Tempos iniciais em segundos; devem ser medidos no protótipo. Ataques usam estados perceber → aproximar → antecipar → executar → recuperar. Ataque mantém a direção fixada ao terminar antecipação; dano uma vez por alvo por execução. Morte cancela ataques e invocações pendentes.

| Encontro | Padrão e antecipação | Recuperação / contraponto |
|---|---|---|
| Lobo Etéreo | Uivo e faixa de investida, 0,7 s | 0,9 s após choque; não atravessa paredes |
| Cultista Cego-Guiado | Selo circular de lentidão, 1 s; projétil visível | 1,2 s; não lança através de obstáculos |
| Garras de Musgo / mapa 01 | Varredura em arco, 0,9 s; investida reta, 1,1 s | 1 s; pilares bloqueiam a investida |
| Garras abaixo de 50% | Raízes em duas áreas alternadas, 1,2 s | Mantém corredor livre; sem reduzir telegraphs |
| Matriarca / mapa 02, fase 1 | Garra frontal, 0,8 s; invoca até quatro larvas ativas | 1 s; larvas têm antecipação própria |
| Matriarca abaixo de 50% | Jato de ácido em cone, 1,2 s; poças temporárias | 1,4 s; limitar área ocupada e preservar saída |
| Silvano / mapa 03, fase 1 | Cajado-espada em arco, 0,9 s; linha de raízes, 1,1 s | 1,1 s; raízes surgem em sequência legível |
| Silvano abaixo de 50% | Três selos sequenciais, 1,2 s cada; onda radial com abertura | 1,5 s; nunca sobrepor padrões sem rota segura |

Mudança de fase: acionada uma única vez ao cruzar 50% de HP, conclui/cancela o ataque atual com regra explícita e concede intervalo seguro. Matriarca muda de azul para vermelho junto com postura, áudio e padrão; cor sozinha não comunica fase. Invocações são limpas no reset. Arenas não dependem de habilidade de uma classe específica.

## Confronto e transições

Silvano, antes da luta: “Essa luz em sua mão… não. Ele não pisaria aqui outra vez.”

Após a luta: “A marca em sua alma… você carrega a autoridade Dele.” O fragmento se desprende; uma raiz forma um brasão incompleto e se desfaz. A decisão As Raízes do Guardião oferece selar ou drenar a seiva, conforme a campanha.

I → II: “Além das árvores, o vento não trazia folhas. Trazia sal — e nomes raspados da pedra.”

II → III: “Sob as tumbas, algo ainda trabalhava. Nenhum dos mortos lembrava para quem.”

III → IV: “A escada subia onde já não havia céu. O cristal em sua bolsa pulsava antes de cada passo.”

As transições respeitam fatos registrados no save e não pressupõem uma escolha específica. Diálogos posteriores variam por decisão e conhecimento desbloqueado.
