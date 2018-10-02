import png, array
from random import randint
import math

import sys

if len(sys.argv) != 2:
	print("Give size of case. E.g. python InputCreator.py 1000")
	sys.exit(1)


NUMBER = int(sys.argv[1])

reader = png.Reader(filename='duck.png')
w, h, pixels, metadata = reader.read_flat()



pixel_byte_width = 4 if metadata['alpha'] else 3
coordinates = []

coordinates = set([])
xcoordinates = []
ycoordinates = []
while len(coordinates) < NUMBER:
	x = randint(1, w-1)
	y = randint(1, h-1)
	pixel_position = x + y * w
	
	if(pixels[ pixel_position * pixel_byte_width ] != 255) and pixel_position not in coordinates:
		cando = True
		for i in range(len(xcoordinates)):
			if ( math.sqrt(math.pow(x-xcoordinates[i],2) + math.pow(y-ycoordinates[i],2))) < 3:
				cando = False
				continue
		if cando:
			coordinates.add(pixel_position)
			xcoordinates.append(x)
			ycoordinates.append(y)
			print ( len(coordinates) )





with open("input%s.csv"%NUMBER, "w") as text_file:
	for i in range(NUMBER):
		text_file.write("%s,%s,%s\n" % (i,xcoordinates[i],ycoordinates[i]))
    

