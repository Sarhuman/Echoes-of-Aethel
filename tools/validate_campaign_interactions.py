"""Validate editorial coverage, not Unity gameplay or approved canon."""
import argparse
import itertools
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SPEC = ROOT / 'docs/design/campaign-interaction-spec.json'
ROUTE = ROOT / 'docs/engineering/campaign-route-manifest.json'
OUTPUT = ROOT / 'docs/design/campaign-interactions.md'
MAP_FIELDS = ['identity', 'main_interaction', 'optional_discovery', 'human_anchor', 'combat_role', 'readability', 'return_reaction', 'pacing', 'reward_role', 'regional_hub', 'acceptance', 'consequence_authority']


def require(condition, message):
    if not condition:
        raise ValueError(message)


def validate(spec, route):
    require(spec['status'] == 'editorial_proposal_not_runtime', 'Status must distinguish proposal from runtime')
    require(spec['proposal_ids_are_runtime_ids'] is False, 'Proposal IDs cannot become runtime IDs')
    canonical = route['maps']
    require(len(canonical) == len(spec['maps']) == 12, 'Exactly 12 campaign maps')
    require(len({m['zone_id'] for m in spec['maps']}) == 12, 'Duplicate zone')
    require(len(spec['hubs']) == 4 and len({h['zone_id'] for h in spec['hubs']}) == 4, 'Four distinct hubs')
    for hub in spec['hubs']:
        require(hub['safe'] is True, 'Unsafe hub')
        require(set(hub['services']) >= {'preparation', 'save', 'inventory', 'essential_shop'}, 'Missing essential service')
        require(len(hub['interactions']) >= 3, 'Hub has no meaningful interactions')
    for index, (actual, source) in enumerate(zip(spec['maps'], canonical)):
        for field in ['zone_id', 'name', 'act', 'quest_id', 'choice_ids', 'final_encounter', 'requires_previous_zone', 'unlocks_zone']:
            require(actual[field] == source[field], f'Canonical mismatch {index + 1}: {field}')
        require(len(actual['choice_ids']) == 2 and len(set(actual['choice_ids'])) == 2, 'Two distinct choices required')
        require(actual['implemented_unity'] is False, 'Cannot claim implementation')
        require(all(isinstance(actual.get(f), str) and actual[f].strip() for f in MAP_FIELDS), 'Incomplete region')
        require(actual['regional_hub'] == spec['hubs'][(index // 3)]['name'], 'Wrong regional hub')
        expected = canonical[index-1]['zone_id'] if index else None
        require(actual['requires_previous_zone'] == expected, 'Broken prior-zone gate')
        following = canonical[index+1]['zone_id'] if index < 11 else None
        require(actual['unlocks_zone'] == following, 'Broken next-zone gate')
        if index in (2, 5, 8, 11):
            require(actual['decision_timing'] == 'after_victory', 'Boss decision must follow victory')
    require(len(spec['dungeons']) == 2, 'Two optional dungeons')
    expected_dungeons = {'A Coroa de Sucata': 'dunes_04', 'O Livro dos Nomes Perdidos': 'crypt_06'}
    require({d['name']: d['source_zone'] for d in spec['dungeons']} == expected_dungeons, 'Dungeon identity/entrance mismatch')
    for dungeon in spec['dungeons']:
        require(dungeon['optional'] and not dungeon['campaign_fragment'] and not dungeon['affects_ritual_charge'], 'Dungeon cannot gate campaign or ritual')
        require(not dungeon['runtime_ids_approved'], 'New IDs require editorial approval')
        require(dungeon['category'] == 'sub_boss', 'Dungeon category mismatch')
        require(len(dungeon['beats']) >= 8 and len(dungeon['acceptance']) >= 5, 'Incomplete dungeon')
        for field in ['identity', 'discovery', 'boss_answer', 'narrative_payoff', 'reward_policy', 'replay_policy', 'entry_exit']:
            require(bool(dungeon[field].strip()), 'Missing dungeon contract')
    require(len(spec['side_quests']) == 3, 'Three narrative side quests')
    zones = {m['zone_id']: m for m in canonical}
    for quest in spec['side_quests']:
        require(quest['zone'] in zones and zones[quest['zone']]['act'] == quest['act'], 'Side quest act mismatch')
        require(not quest['affects_ritual_charge'] and len(quest['options']) == 2, 'Side quest ritual/options mismatch')
    keys = [x['proposal_key'] for x in spec['dungeons'] + spec['side_quests']]
    require(len(set(keys)) == len(keys) and all(k.startswith('proposal.') for k in keys), 'Proposal keys duplicate or unmarked')
    # Exhaustive traversal of the DESIGN gate, not a gameplay/save test.
    paths = 0
    for options in itertools.product(*[m['choice_ids'] for m in canonical]):
        completed = set()
        for m, option in zip(canonical, options):
            require(m['requires_previous_zone'] is None or m['requires_previous_zone'] in completed, 'Unreachable route')
            require(option in m['choice_ids'], 'Invalid choice')
            completed.add(m['zone_id'])
        require(len(completed) == 12, 'Incomplete design path')
        paths += 1
    return paths


def render(spec):
    lines = ['# Campanha inteira — interações, pessoas e retornos', '',
             '> PROPOSTA EDITORIAL, não implementação. Gerado de `campaign-interaction-spec.json`; editar o JSON e executar `python tools/validate_campaign_interactions.py --write`.', '',
             'As doze quests, opções, encontros e gates permanecem os do manifesto canônico. Interações, encenação, papéis táticos e novos nomes abaixo são NOVO/EXPERIMENTAL. Consequências e seus gatilhos exatos permanecem no GDD 04; não antecipar efeitos tardios. Os nomes propostos dos NPCs não são IDs de save.', '',
             'Meta: cada região oferece uma ação característica, pessoa ou testemunho, descoberta opcional, confronto legível e retorno reconhecível. Igual atenção de design não significa igual duração ou quantidade de puzzles. O Coração não recebe conteúdo de preenchimento.', '',
             '## Regiões', '']
    labels = ['Identidade','Interação principal','Descoberta opcional','Âncora humana','Papel dos encontros','Leitura e acessibilidade','Retorno e consequência','Ritmo','Função da recompensa','Hub regional','Aceite','Autoridade das consequências']
    for m in spec['maps']:
        lines += [f"### {m['name']} — `{m['zone_id']}`", '', f"Quest `{m['quest_id']}`; opções `{m['choice_ids'][0]}` / `{m['choice_ids'][1]}`. Confronto: **{m['final_encounter']['name']}**. Decisão: `{m['decision_timing']}`.", '']
        for field, label in zip(MAP_FIELDS, labels): lines += [f"**{label}:** {m[field]}", '']
    lines += ['## Hubs com vida própria', '']
    for h in spec['hubs']:
        lines += [f"### {h['name']} — `{h['zone_id']}`", '', h['cast'], '', h['proposed_local_role'], '']
        lines += ['- '+s for s in h['interactions']]
        lines += ['', '**Memória regional:** '+h['regional_memory'], '', 'Sempre seguros; preparação, save, inventário e loja essencial preservados.', '']
    lines += ['## Duas dungeons completas em proposta', '']
    for d in spec['dungeons']:
        lines += [f"### {d['name']}", '', f"Entrada: `{d['source_zone']}`. Subchefe: **{d['boss']}**. Referência editorial: `{d['proposal_key']}`; não usar como ID de runtime.", '', '**Identidade:** '+d['identity'], '', '**Descoberta:** '+d['discovery'], '']
        lines += [f'{i}. {beat}' for i,beat in enumerate(d['beats'],1)]
        for key,label in [('boss_answer','Resposta do jogador'),('narrative_payoff','Resultado narrativo'),('reward_policy','Recompensa'),('replay_policy','Reentrada'),('entry_exit','Entrada e saída')]: lines += ['',f'**{label}:** {d[key]}']
        lines += ['', '**Aceites:**', '']+['- '+a for a in d['acceptance']]+['']
    lines += ['## Secundárias narrativas', '']
    for q in spec['side_quests']:
        lines += [f"### {q['name']}", '', f"Local proposto: `{q['zone']}`. Referência editorial: `{q['proposal_key']}`. Sem carga ritual.", '', q['setup'], '', q['interaction'], '']+['- '+v for v in q['options']]+['', '**Retorno:** '+q['return_to'], '']
    lines += ['## Ainda não fechado', '', 'A validação confirma cobertura estrutural e preservação do manifesto. Não aprova novos nomes, balanceamento, planta, assets, código ou gameplay. Gates restantes:', '']+['- '+g for g in spec['open_gates']]+['']
    return '\n'.join(lines)


def main():
    cli=argparse.ArgumentParser(description=__doc__)
    cli.add_argument('--write', action='store_true')
    args=cli.parse_args()
    spec=json.loads(SPEC.read_text(encoding='utf-8'))
    route=json.loads(ROUTE.read_text(encoding='utf-8'))
    paths=validate(spec,route)
    rendered=render(spec)
    if args.write: OUTPUT.write_text(rendered,encoding='utf-8')
    else: require(OUTPUT.read_text(encoding='utf-8')==rendered,'Generated markdown is stale; run --write')
    print(f'DESIGN ONLY: 12 maps, 4 hubs, 2 dungeons, 3 side quests; {paths} choice paths; canonical references preserved.')

if __name__ == '__main__': main()
