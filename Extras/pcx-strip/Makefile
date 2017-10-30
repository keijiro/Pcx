TARGET = pcx-strip

SRCS = pcx-strip.cpp tinyply.cpp
OBJS = $(SRCS:.cpp=.o)

CXXFLAGS = -O2 -Wall -Wno-reorder -std=c++11
LDFLAGS =
LIBS = -lstdc++fs

ifeq ($(PLATFORM), Windows)
	BIN_PREFIX = x86_64-w64-mingw32
	OUTPUT = $(TARGET).exe
else
	BIN_PREFIX =
	OUTPUT = $(TARGET)
endif

CXX = $(BIN_PREFIX)-c++
STRIP = $(BIN_PREFIX)-strip

all: $(OUTPUT)

strip: $(OUTPUT)
	$(STRIP) $(OUTPUT)

clean:
	rm -f $(OUTPUT) $(OBJS)

$(OUTPUT): $(OBJS)
	$(CXX) $(LDFLAGS) -o $(OUTPUT) $(OBJS) $(LIBS)

.cpp.o:
	$(CXX) $(CXXFLAGS) -c -o $@ $<
