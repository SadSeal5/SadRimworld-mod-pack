import os
from PIL import Image, ImageDraw, ImageFont

def render_banner():
    # Supersampling 2x for ultra-sharp rendering
    SCALE = 2
    W, H = 800 * SCALE, 450 * SCALE

    img = Image.new("RGBA", (W, H), (14, 20, 32, 255))
    draw = ImageDraw.Draw(img)

    # Load Windows fonts
    font_dir = "C:/Windows/Fonts"
    font_title = ImageFont.truetype(os.path.join(font_dir, "segoeuib.ttf"), 30 * SCALE)
    font_sub = ImageFont.truetype(os.path.join(font_dir, "segoeuib.ttf"), 12 * SCALE)
    font_card_title = ImageFont.truetype(os.path.join(font_dir, "segoeuib.ttf"), 13 * SCALE)
    font_card_sub = ImageFont.truetype(os.path.join(font_dir, "segoeui.ttf"), 9 * SCALE)
    font_bullet = ImageFont.truetype(os.path.join(font_dir, "segoeui.ttf"), 9 * SCALE)
    font_badge = ImageFont.truetype(os.path.join(font_dir, "segoeuib.ttf"), 11 * SCALE)

    # Subtle background gradient/vignette
    for y in range(H):
        alpha = int(14 + (y / H) * 8)
        draw.line([(0, y), (W, y)], fill=(alpha, alpha + 6, alpha + 18, 255))

    # Outer double border
    pad1 = 8 * SCALE
    draw.rectangle([pad1, pad1, W - pad1, H - pad1], outline=(37, 69, 107, 255), width=2 * SCALE)
    pad2 = 12 * SCALE
    draw.rectangle([pad2, pad2, W - pad2, H - pad2], outline=(22, 42, 66, 255), width=1 * SCALE)

    # Top Header Box
    header_y = 32 * SCALE
    title_text = "SADSEAL'S MEGA MOD"
    title_bbox = draw.textbbox((0, 0), title_text, font=font_title)
    title_w = title_bbox[2] - title_bbox[0]
    draw.text(((W - title_w) // 2, header_y), title_text, font=font_title, fill=(255, 255, 255, 255))

    sub_y = header_y + 38 * SCALE
    sub_text = "LETHAL COMBAT  •  SKILL MULTIPLIERS  •  RECIPE TWEAKER  •  GENE SYNTHESIZER"
    sub_bbox = draw.textbbox((0, 0), sub_text, font=font_sub)
    sub_w = sub_bbox[2] - sub_bbox[0]
    draw.text(((W - sub_w) // 2, sub_y), sub_text, font=font_sub, fill=(86, 162, 232, 255))

    # 4 Cards configuration
    cards = [
        {
            "title": "Lethal Combat",
            "subtitle": "Ballistics & Stagger",
            "border": (224, 83, 83, 255),
            "title_color": (255, 107, 107, 255),
            "header_bg": (42, 24, 32, 255),
            "card_bg": (18, 24, 38, 255),
            "bullets": [
                "Universal Ballistic Accuracy",
                "Scalable Damage & AP",
                "High-Velocity Projectiles",
                "Kinetic Stagger & Slow",
                "1-HP Preservation Bypass",
                "Instant Balance Presets"
            ]
        },
        {
            "title": "Skill Multipliers",
            "subtitle": "Learning & Decay Control",
            "border": (56, 139, 253, 255),
            "title_color": (88, 166, 255, 255),
            "header_bg": (22, 34, 56, 255),
            "card_bg": (18, 24, 38, 255),
            "bullets": [
                "0.0x to 100.0x XP Scaling",
                "Global & Per-Skill Sliders",
                "Disable 4k/Day Soft-Cap",
                "Disable Skill 10+ Decay",
                "Colonists-Only Filter",
                "Instant Quick Presets"
            ]
        },
        {
            "title": "Recipe Tweaker",
            "subtitle": "In-Game Recipe & Crafting",
            "border": (227, 179, 65, 255),
            "title_color": (242, 204, 96, 255),
            "header_bg": (42, 36, 22, 255),
            "card_bg": (18, 24, 38, 255),
            "bullets": [
                "Full In-Game Recipe Editor",
                "Create Custom Recipes",
                "Workbench & Tech Picker",
                "Custom Material Ingredients",
                "Adjust Craft Speed & Amounts",
                "One-Click XML Exporter"
            ]
        },
        {
            "title": "Gene Synthesizer",
            "subtitle": "Biotech Gene Crafting",
            "border": (188, 140, 255, 255),
            "title_color": (210, 168, 255, 255),
            "header_bg": (36, 26, 52, 255),
            "card_bg": (18, 24, 38, 255),
            "bullets": [
                "Craft Any Vanilla & Mod Gene",
                "Gene Assembler Integration",
                "Configurable Neutro Cost",
                "Custom Work Time / Instant",
                "Archite Capsule Options",
                "Search & Category Filters"
            ]
        }
    ]

    card_y = 120 * SCALE
    card_h = 265 * SCALE
    margin_x = 24 * SCALE
    spacing = 10 * SCALE
    total_w = W - (margin_x * 2)
    card_w = (total_w - (spacing * 3)) // 4

    for i, c in enumerate(cards):
        cx = margin_x + i * (card_w + spacing)
        cy = card_y

        # Draw card background
        draw.rounded_rectangle([cx, cy, cx + card_w, cy + card_h], radius=4 * SCALE, fill=c["card_bg"], outline=c["border"], width=1 * SCALE)

        # Header bar background
        header_h = 42 * SCALE
        draw.rounded_rectangle([cx, cy, cx + card_w, cy + header_h], radius=4 * SCALE, fill=c["header_bg"])
        # Cover bottom corners of header to keep it flush
        draw.rectangle([cx, cy + header_h - 4 * SCALE, cx + card_w, cy + header_h], fill=c["header_bg"])
        draw.line([(cx, cy + header_h), (cx + card_w, cy + header_h)], fill=c["border"], width=1 * SCALE)

        # Header Title
        tb = draw.textbbox((0, 0), c["title"], font=font_card_title)
        tw = tb[2] - tb[0]
        draw.text((cx + (card_w - tw) // 2, cy + 6 * SCALE), c["title"], font=font_card_title, fill=c["title_color"])

        # Header Subtitle
        sb = draw.textbbox((0, 0), c["subtitle"], font=font_card_sub)
        sw = sb[2] - sb[0]
        draw.text((cx + (card_w - sw) // 2, cy + 24 * SCALE), c["subtitle"], font=font_card_sub, fill=(160, 168, 184, 255))

        # Bullets
        by = cy + header_h + 10 * SCALE
        bullet_gap = 18 * SCALE
        for b in c["bullets"]:
            bullet_char = "•"
            draw.text((cx + 10 * SCALE, by), bullet_char, font=font_bullet, fill=c["title_color"])
            draw.text((cx + 20 * SCALE, by), b, font=font_bullet, fill=(215, 222, 232, 255))
            by += bullet_gap

    # Bottom Right Ready Badge
    badge_text = "RimWorld 1.5, 1.6 & 1.7 (Beta) Ready"
    badge_bbox = draw.textbbox((0, 0), badge_text, font=font_badge)
    badge_tw = badge_bbox[2] - badge_bbox[0]
    badge_th = badge_bbox[3] - badge_bbox[1]

    badge_pad_x = 12 * SCALE
    badge_pad_y = 6 * SCALE
    badge_w = badge_tw + (badge_pad_x * 2)
    badge_h = badge_th + (badge_pad_y * 2)

    badge_x = W - margin_x - badge_w
    badge_y = H - 38 * SCALE

    draw.rounded_rectangle(
        [badge_x, badge_y, badge_x + badge_w, badge_y + badge_h],
        radius=4 * SCALE,
        fill=(28, 89, 54, 255),
        outline=(46, 164, 79, 255),
        width=1 * SCALE
    )
    draw.text((badge_x + badge_pad_x, badge_y + badge_pad_y - 1 * SCALE), badge_text, font=font_badge, fill=(255, 255, 255, 255))

    # Downsample with high quality Lanczos filter for smooth text antialiasing
    final_img = img.resize((800, 450), Image.Resampling.LANCZOS)
    final_img.save("About/Preview.png", "PNG", optimize=True)
    print("Banner generated successfully at About/Preview.png")

if __name__ == "__main__":
    render_banner()
