# WPF 组件状态检查清单

> 每个 UI 组件设计时需覆盖的完整状态。
> 使用 VisualStateManager 或 Trigger 实现。

---

## 1. Button

| 状态 | 检查项 | WPF 实现方式 |
|------|--------|-------------|
| ✅ Normal | 默认背景/前景/边框 | `<Trigger Property="IsMouseOver" Value="False">` |
| ✅ MouseOver | 悬停颜色变化、光标 | `<Trigger Property="IsMouseOver" Value="True">` |
| ✅ Pressed | 按下时视觉反馈（缩放/颜色加深） | `<Trigger Property="IsPressed" Value="True">` |
| ✅ Disabled | 灰色、不可交互外观 | `<Trigger Property="IsEnabled" Value="False">` |
| ✅ Focused | 聚焦边框或发光效果 | `<Trigger Property="IsFocused" Value="True">` |

**HandyControl 特定**：
```xml
<!-- 圆角 -->
<Button hc:BorderElement.CornerRadius="8"/>

<!-- 图标按钮 -->
<Button Content="保存" hc:IconElement.Geometry="{StaticResource SaveGeometry}"/>
```

---

## 2. TextBox / WatermarkTextBox

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Normal | 默认边框/背景 | — |
| ✅ Focused | 聚焦高亮边框 | `<Trigger Property="IsFocused" Value="True">` |
| ✅ Disabled | 灰色只读外观 | `<Trigger Property="IsEnabled" Value="False">` |
| ✅ ReadOnly | 背景透明/浅色 | `<Trigger Property="IsReadOnly" Value="True">` |
| ✅ Watermark | 占位文字显示 | HandyControl WatermarkTextBox（通过 Tag 传入占位文字） |

**HandyControl 特定**：
```xml
<!-- Watermark 文本框 -->
<TextBox Style="{StaticResource WatermarkTextBox}" Tag="请输入参数值"/>
```

---

## 3. ComboBox

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Normal | 默认外观 | — |
| ✅ DropDownOpen | 下拉展开时边框/背景变化 | `<Trigger Property="IsDropDownOpen" Value="True">` |
| ✅ Selection | 选中项高亮 | ItemContainerStyle 中的 `<Trigger Property="IsSelected" Value="True">` |
| ✅ Disabled | 灰色不可用 | `<Trigger Property="IsEnabled" Value="False">` |

---

## 4. CheckBox

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Unchecked | 未选中外观 | `<Trigger Property="IsChecked" Value="False">` |
| ✅ Checked | 选中后勾选标记 | `<Trigger Property="IsChecked" Value="True">` |
| ✅ Indeterminate | 不确定状态（方框） | `<Trigger Property="IsChecked" Value="{x:Null}">` |
| ✅ Disabled | 灰色不可交互 | `<Trigger Property="IsEnabled" Value="False">` |

---

## 5. RadioButton

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Unchecked | 未选中外观 | `<Trigger Property="IsChecked" Value="False">` |
| ✅ Checked | 选中后圆心填充 | `<Trigger Property="IsChecked" Value="True">` |
| ✅ Disabled | 灰色不可交互 | `<Trigger Property="IsEnabled" Value="False">` |

**HandyControl 特定（Icon 模式）**：
```xml
<RadioButton Style="{StaticResource RadioButtonIcon}"
             hc:VisualElement.HighlightBrush="Red"/>
```

---

## 6. DataGrid

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Normal | 默认行外观 | Row Style 默认 |
| ✅ RowHover | 行悬停高亮 | `<Trigger Property="IsMouseOver" Value="True">` on DataGridRow |
| ✅ RowSelected | 行选中高亮 | `<Trigger Property="IsSelected" Value="True">` |
| ✅ CellEditing | 编辑中单元格外观 | `<Trigger Property="IsEditing" Value="True">` |
| ✅ AlternatingRow | 斑马纹 | `AlternatingRowBackground` 属性 |

**示例**：
```xml
<DataGrid AlternatingRowBackground="#F5F5F5"
          RowBackground="#FFFFFF"
          AutoGenerateColumns="False">
    <DataGrid.RowStyle>
        <Style TargetType="DataGridRow">
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter Property="Background" Value="#E8F0FE"/>
                </Trigger>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="#D0E0FF"/>
                </Trigger>
            </Style.Triggers>
        </Style>
    </DataGrid.RowStyle>
</DataGrid>
```

---

## 7. TabControl

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ TabHover | 标签悬停 | TabItem Header 的 MouseOver 触发器 |
| ✅ TabSelected | 标签选中（高亮/下划线） | `<Trigger Property="IsSelected" Value="True">` |
| ✅ TabDisabled | 禁用标签 | `<Trigger Property="IsEnabled" Value="False">` |
| ✅ Content | 内容区域背景/边框 | TabControl 的 ContentTemplate |

---

## 8. ListBox / ListView

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ ItemHover | 项悬停 | `<Trigger Property="IsMouseOver" Value="True">` |
| ✅ Selected | 项选中 | `<Trigger Property="IsSelected" Value="True">` |
| ✅ Focused | 项聚焦 | `<Trigger Property="IsFocused" Value="True">` |
| ✅ Disabled | 列表禁用 | `<Trigger Property="IsEnabled" Value="False">` |

---

## 9. Border / Card（分组容器）

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Normal | 默认边框/背景/圆角 | — |
| ✅ Hover | （可选）悬停时边框变色或阴影上浮 | 通过 Style 添加 MouseOver 触发 |
| ✅ Active | （可选）选中/活动时边框高亮 | 通过数据绑定或 Tag 驱动 |

**HandyControl 特定**：
```xml
<Border hc:BorderElement.CornerRadius="10"
        BorderBrush="LightGray"
        Background="White"
        Padding="8"/>
```

---

## 10. Slider

| 状态 | 检查项 | 说明 |
|------|--------|------|
| ✅ Normal | 默认轨道/拇指外观 | — |
| ✅ ThumbDrag | 拖动时拇指放大/变色 | `<Trigger Property="IsDragging" Value="True">` on Thumb |
| ✅ Disabled | 灰色不可交互 | `<Trigger Property="IsEnabled" Value="False">` |

---

## 通用检查清单

### 每个页面/组件在完成后检查：

- [ ] **所有控件覆盖上述 5 种状态**（Normal/MouseOver/Pressed/Disabled/Focused）
- [ ] **色彩一致性**：所有 Brand 色使用资源引用，无硬编码
- [ ] **圆角一致性**：使用统一的 CornerRadius 资源
- [ ] **间距一致性**：使用统一的 Thickness 资源（Margin/Padding）
- [ ] **字体一致性**：使用统一的 FontSize 资源
- [ ] **键盘导航**：TabIndex 顺序合理，可见的 FocusVisualStyle
- [ ] **HandyControl 检查**：所有可用的附加属性已利用（CornerRadius、IconElement 等）
- [ ] **无冗余样式**：重复的属性值提取为 Style
- [ ] **窗口缩放**：内容不溢出，ScrollViewer 在需要处添加
