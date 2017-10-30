// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

#include <fstream>
#include <iostream>
#include <experimental/filesystem>
#include "tinyply.h"

namespace fs = std::experimental::filesystem;

int main(int argc, char* argv[])
{
    if (argc < 2)
    {
        std::cout << "Usage: pcx-strip [FILE]" << std::endl;
        return 0;
    }

    // Source/Output file path
    const auto src_path = fs::path(argv[1]);
    const auto out_path = fs::path(src_path).
        replace_filename(src_path.stem().string() + "-stripped.ply");

    try
    {
        // Open file streams.
        std::ifstream src_stream(src_path, std::ios::binary);
        std::ofstream out_stream(out_path, std::ios::binary | std::ios::trunc);
        if (src_stream.fail()) throw std::runtime_error("can't open file: " + src_path.string());
        if (out_stream.fail()) throw std::runtime_error("can't open file: " + out_path.string());

        // Parse the head of the source file.
        tinyply::PlyFile src_ply;
        src_ply.parse_header(src_stream);

        // Vertex positions
        auto vertices = src_ply.request_properties_from_element("vertex", { "x", "y", "z" });

        // Vertex colors (read only if it exists)
        std::shared_ptr<tinyply::PlyData> colors;

        try
        {
            colors = src_ply.request_properties_from_element("vertex", { "red", "green", "blue" });
        }
        catch (const std::exception & e)
        {
            colors.reset();
        }

        // Read data body.
        src_ply.read(src_stream);

        // Output stripped data.
        tinyply::PlyFile out_ply;

        out_ply.add_properties_to_element(
            "vertex", { "x", "y", "z" },
            tinyply::Type::FLOAT32, vertices->count * 3, vertices->buffer.get(),
            tinyply::Type::INVALID, 0
        );

        if (colors)
            out_ply.add_properties_to_element(
                "vertex", { "red", "green", "blue" },
                tinyply::Type::UINT8, colors->count * 3, colors->buffer.get(),
                tinyply::Type::INVALID, 0
            );

        out_ply.write(out_stream, true);
    }
    catch (const std::exception& e)
    {
        std::cerr << "Error: " << e.what() << std::endl;
    }

    return 0;
}
