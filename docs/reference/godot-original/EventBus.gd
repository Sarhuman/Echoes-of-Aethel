# core/EventBus.gd
# Barramento de Eventos Global (Singleton / Autoload)
# Implementação do Padrão Observer para desacoplamento total entre sistemas.

class_name EventBusNode

# --- SINAIS DE COMBATE E SAÚDE ---
signal actor_damaged(target_actor: Node, damage_amount: int, attacker: Node)
signal actor_died(dead_actor: Node)

# --- SINAIS DE DIÁLOGO E ESCOLHAS ---
signal dialogue_started(npc_id: String)
signal dialogue_option_selected(option_id: String, consequences: Dictionary)
signal dialogue_ended(npc_id: String)

# --- SINAIS DE MUNDO E CRIME ---
signal crime_committed(victim_id: String, crime_type: String)
signal zone_changed(new_zone_id: String)

# --- SINAIS DE INVENTÁRIO E LOOT ---
signal item_picked_up(item_data: Resource, quantity: int)
signal item_used(item_data: Resource)
