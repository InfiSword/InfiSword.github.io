import zipfile
import xml.etree.ElementTree as ET
import re
import os

pptx_path = r"C:\Ptf\InfiSword.github.io\Context\리얼 포폴.pptx"

def get_slide_text(xml_content):
    root = ET.fromstring(xml_content)
    texts = []
    for elem in root.iter():
        if elem.tag.endswith('}t'): # <a:t> tag
            if elem.text:
                texts.append(elem.text.strip())
    return texts

def main():
    if not os.path.exists(pptx_path):
        print(f"File not found: {pptx_path}")
        return
        
    # Get scratch directory path
    scratch_dir = os.path.dirname(os.path.abspath(__file__))
    output_path = os.path.join(scratch_dir, "pptx_analysis.txt")
        
    with zipfile.ZipFile(pptx_path, 'r') as archive:
        slide_files = [f for f in archive.namelist() if re.match(r'ppt/slides/slide\d+\.xml', f)]
        slide_files.sort(key=lambda f: int(re.search(r'\d+', f).group()))
        
        with open(output_path, "w", encoding="utf-8") as out:
            out.write(f"Total Slides: {len(slide_files)}\n\n")
            
            for slide_file in slide_files:
                slide_num = re.search(r'\d+', slide_file).group()
                xml_content = archive.read(slide_file)
                texts = get_slide_text(xml_content)
                
                out.write(f"=== Slide {slide_num} ===\n")
                if texts:
                    clean_texts = [t for t in texts if t]
                    out.write(" | ".join(clean_texts) + "\n")
                else:
                    out.write("[No Text Content]\n")
                out.write("\n")
                
        print(f"Saved analysis to {output_path}")

if __name__ == "__main__":
    main()
