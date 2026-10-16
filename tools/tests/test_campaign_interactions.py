import importlib.util
import json
from pathlib import Path
import unittest

ROOT=Path(__file__).resolve().parents[2]
spec=importlib.util.spec_from_file_location('coverage',ROOT/'tools/validate_campaign_interactions.py')
m=importlib.util.module_from_spec(spec)
spec.loader.exec_module(m)

class CampaignCoverageTests(unittest.TestCase):
    def setUp(self):
        self.data=json.loads(m.SPEC.read_text(encoding='utf-8'))
        self.route=json.loads(m.ROUTE.read_text(encoding='utf-8'))
    def rejected(self):
        with self.assertRaises(ValueError): m.validate(self.data,self.route)
    def test_complete(self): self.assertEqual(m.validate(self.data,self.route),4096)
    def test_missing_region(self): self.data['maps'].pop(); self.rejected()
    def test_changed_canonical_choice(self): self.data['maps'][1]['choice_ids'][0]='new'; self.rejected()
    def test_incomplete_interaction(self): self.data['maps'][7]['main_interaction']=''; self.rejected()
    def test_dungeon_required(self): self.data['dungeons'][0]['optional']=False; self.rejected()
    def test_wrong_dungeon_entrance(self): self.data['dungeons'][1]['source_zone']='forest_01'; self.rejected()
    def test_ritual_contamination(self): self.data['side_quests'][0]['affects_ritual_charge']=True; self.rejected()
    def test_unsafe_hub(self): self.data['hubs'][2]['safe']=False; self.rejected()
    def test_missing_service(self): self.data['hubs'][3]['services'].remove('save'); self.rejected()
    def test_final_choice_before_victory(self): self.data['maps'][11]['decision_timing']='before_exit'; self.rejected()
    def test_false_implementation_claim(self): self.data['maps'][0]['implemented_unity']=True; self.rejected()
    def test_duplicate_proposal_keys(self): self.data['dungeons'][1]['proposal_key']=self.data['dungeons'][0]['proposal_key']; self.rejected()
    def test_generated_doc_matches(self): self.assertEqual(m.render(self.data),m.OUTPUT.read_text(encoding='utf-8'))

if __name__=='__main__': unittest.main()
