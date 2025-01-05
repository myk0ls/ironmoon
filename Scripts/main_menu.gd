extends Control

@onready var play = get_node("MarginContainer/VBoxContainer/Play")
@onready var settings = get_node("MarginContainer/VBoxContainer/Settings")
@onready var exit = get_node("MarginContainer/VBoxContainer/Exit")

@onready var level0 = preload("res://Scenes/Levels/level0.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	MusicManager.Play("MainMenu")
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass



func _on_play_pressed():
	MusicManager.Stop("MainMenu")
	get_tree().change_scene_to_packed(level0)


func _on_exit_pressed():
	get_tree().quit()
