import argparse
import os
import math

Tiles = [
    [0, 0, 0],       #nothing
    [255, 255, 255], #block
    [0, 85, 0],        #ponkotsu spawn
    [255, 0, 0]         #bonkura spawn
]

parser = argparse.ArgumentParser(description="Process some integers.")
parser.add_argument("-f", "--files", type=str, nargs="+", help="files to be processed", action="extend")
parser.add_argument("-s", "--size", type=str, default="7x7x7", help="set level size")
#parser.add_argument("-s", "--size", type=str, nargs="+", default="7x7x7", help="set level size", action="extend")
parser.add_argument("-d", "--dest", type=str, default=".\\", help="set file destination folder")
args = parser.parse_args()

def get_size(s:str):
    list = s.split("x")

    if (len(list) != 3):
        return 0
    return {"x": int(list[0]), "y": int(list[1]), "z": int(list[2])}

def read_file(path):
    file = open(path, "r")
    header = True
    size = get_size(args.size)
    half = {"x": math.floor(size["x"]/2), "y": math.floor(size["y"]/2), "z": math.floor(size["z"]/2)}
    data:str = [chr(0)] * (size["x"] * size["y"] * size["z"])
    index = 0
    for line in file:
        if header:
            if line == "end_header\n":
                header = False
        else:
            list = line[:-1].split(" ")
            list = [int(x) for x in list]
            list[0] += half["x"]
            list[1] += half["y"]
            value = 0
            for tile in Tiles:
                if list[3] == tile[0] and list[4] == tile[1] and list[5] == tile[2]:
                    data[index] = chr(value)
                value += 1
            index += 1
    file.close()
    return "".join(data)

def create_level_file(path, data):
    name = os.path.splitext(os.path.basename(path))[0]
    path = args.dest + name + ".dat"

    file = open(path, "w")
    file.write(args.size + "\n" + data)
    file.close()

if (args.files == None):
    path = input("Enter file path: ")
    size = input("Enter Level size (x y z): ")
    args.size = size
    header = True
    create_level_file(path, read_file(path))
else:
    for path in args.files:
        create_level_file(path, read_file(path))