extends Skeleton3D

var RobotSpider

# Called when the node enters the scene tree for the first time.
func _ready():
	RobotSpider = get_parent().get_parent().get_parent()
	RobotSpider.Death.connect(RagdollStart)
	#connect(RobotSpider.Death, RagdollStart())
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func RagdollStart():
	physical_bones_start_simulation()
