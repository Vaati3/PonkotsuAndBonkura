import argparse
import os
import math

Tiles = [
    [0, 0, 0],       #void
    [255, 255, 255], #block
    [0, 255, 0],     #ponkotsu spawn
    [255, 0, 0],     #bonkura spawn
    [0, 150, 0],     #ponkotsu goal
    [150, 0, 0],     #bonkura goal
    [255, 255, 0],   #elevator X
    [200, 200, 0],   #elevator Y
    [145, 145, 0],   #elevator Z
    [255, 145, 0],   #elvator stop
    [255, 0, 255],   #button X
    [200, 0, 200],   #button Y
    [145, 0, 145],   #button Z
    [0, 0, 255],     #rotator 
    [200, 200, 250], #door
    [255, 0, 100],   #swapper
    [130, 50, 0]     #box
]

items = [
    [100, 100, 200], #key
]

parser = argparse.ArgumentParser(description="Process some integers.")
parser.add_argument("-f", "--files", type=str, nargs="+", help="files to be processed", action="extend")
parser.add_argument("-s", "--size", type=str, default="7x7x7", help="set level size, default:7x7x7")
#parser.add_argument("-s", "--size", type=str, nargs="+", default="7x7x7", help="set level size", action="extend")
parser.add_argument("-d", "--dest", type=str, default="./Map/Maps/", help="set file destination folder")
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
    data = [0] * (size["x"] * size["y"] * size["z"])
    for line in file:
        if header:
            if line == "end_header\n":
                header = False
        else:
            list = line[:-1].split(" ")
            list = [int(x) for x in list]
            list[0] += half["x"]
            list[1] += half["y"]
            isTile = False
            value = 0
            for tile in Tiles:
                if list[3] == tile[0] and list[4] == tile[1] and list[5] == tile[2]:
                    data[list[0] + (list[1] * size["x"]) + (list[2] * size["x"] * size["y"])] = value
                    isTile = False
                    break
                value += 1

            if not isTile:
                value = 0
                for item in items:
                    if list[3] == item[0] and list[4] == item[1] and list[5] == item[2]:
                        data[list[0] + (list[1] * size["x"]) + (list[2] * size["x"] * size["y"])] = 255 - value
                        break
                    value += 1
    file.close()
    return data

def create_level_file(path, data):
    name = os.path.splitext(os.path.basename(path))[0]
    path = args.dest + name + ".dat"
    sizeBytes = []
    size = get_size(args.size)
    sizeBytes += [ord(c) for c in str(size["x"])]
    sizeBytes += [ord("x")]
    sizeBytes += [ord(c) for c in str(size["y"])]
    sizeBytes += [ord("x")]
    sizeBytes += [ord(c) for c in str(size["z"])]
    sizeBytes += [ord("\n")]
    file = open(path, "wb")
    file.write(bytearray(sizeBytes) + bytearray(data))
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